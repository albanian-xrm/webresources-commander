using AlbanianXrm.WebResources.Commander;
using AlbanianXrm.WebResources.Commander.Repositories;
using AlbanianXrm.WebResources.DataModel;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.IO;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AlbanianXrm.WebResources
{
    public class WebResourcesCommander
    {
        private readonly IOrganizationService service;
        private readonly RecyclableMemoryStreamManager memoryStreamManager = new RecyclableMemoryStreamManager();
        internal WebResourcesCommanderOptions options;
        internal Matcher matcher;
        internal WebResourceRepository webResourceRepository;
        internal List<WebResource> webResources = new List<WebResource>();
        internal Dictionary<string, RecyclableMemoryStream> memoryWebResources = new Dictionary<string, RecyclableMemoryStream>();
        internal Dictionary<string, string> tempWebResources = new Dictionary<string, string>();
        internal Dictionary<string, string> localWebResources = new Dictionary<string, string>();
        internal Dictionary<string, Operation> pendingOperations = new Dictionary<string, Operation>();

        public WebResourcesCommander(IOrganizationService service, WebResourcesCommanderOptions options)
        {
            this.service = service;
            this.options = options;
            this.matcher = options.GlobPatternsAsMatcher();
            this.webResourceRepository = new WebResourceRepository(service);
        }

        public void SyncWebresources()
        {
            GetLocalWebResources();
            webResources = webResourceRepository.GetWebResourcesInSolution(options.Solution);
            if (options.UseTempFolder)
            {
                SaveWebResourcesInTempFolder();
            }
            else
            {
                SaveWebResourcesInMemory();
            }
            CalculateOperations();
            ApplyPendingOperations();

        }

        public void ApplyPendingOperations()
        {
            foreach (var operation in pendingOperations.Where(o => o.Value == Operation.Delete))
            {
                webResourceRepository.DeleteWebResource(
                    webResources.Find(w => w.Name.Equals(operation.Key,
                    StringComparison.CurrentCultureIgnoreCase)));
            }
            foreach (var operation in pendingOperations.Where(o => o.Value == Operation.Create))
            {
                string content;
                using (var reader = new StreamReader(localWebResources[operation.Key]))
                {
                    content = Convert.ToBase64String(Encoding.UTF8.GetBytes(reader.ReadToEnd()));                   
                }
                var webResource = new WebResource()
                {
                    Name = operation.Key,
                    WebResourceType = Webresource.GetTypeFromExtension(Path.GetExtension(operation.Key),
                                        allowUnsupported: true),
                    Content =  content,
                };
                webResourceRepository.CreateWebResourceAndAddToSolution(webResource, options.Solution);
            }
        }

        public void CalculateOperations()
        {
            if (options.UseTempFolder)
            {
                foreach (var webResource in localWebResources.Keys.Except(tempWebResources.Keys))
                {
                    pendingOperations.Add(webResource, Operation.Create);
                }
                foreach (var webResource in webResources)
                {
                    if (localWebResources.ContainsKey(webResource.Name))
                    {
                        var checksumLocalFile = FileChecksum.GetSHA1Checksum(localWebResources[webResource.Name]);
                        string checksumWebFile = FileChecksum.GetSHA1Checksum(tempWebResources[webResource.Name]);
                        pendingOperations.Add(webResource.Name, checksumLocalFile.Equals(checksumWebFile) ? Operation.NoAction : Operation.Update);
                    }
                    else
                    {
                        pendingOperations.Add(webResource.Name, webResource.IsManaged == true ? Operation.RemoveFromSolution : Operation.Delete);
                    }
                }
            }
            else
            {
                foreach (var webResource in localWebResources.Keys.Except(memoryWebResources.Keys))
                {
                    pendingOperations.Add(webResource, Operation.Create);
                }
                foreach (var webResource in webResources)
                {
                    if (localWebResources.ContainsKey(webResource.Name))
                    {
                        var checksumLocalFile = FileChecksum.GetSHA1Checksum(localWebResources[webResource.Name]);
                        string checksumWebFile = FileChecksum.GetSHA1Checksum(memoryWebResources[webResource.Name]);
                        pendingOperations.Add(webResource.Name, checksumLocalFile.Equals(checksumWebFile) ? Operation.NoAction : Operation.Update);
                    }
                    else
                    {
                        pendingOperations.Add(webResource.Name, webResource.IsManaged == true ? Operation.RemoveFromSolution : Operation.Delete);
                    }
                }
            }
        }

        public void GetLocalWebResources()
        {
            localWebResources.Clear();
            PatternMatchingResult result = matcher.Execute(
                                         new DirectoryInfoWrapper(
                                             new DirectoryInfo(options.SourceFolder)));
            foreach (var file in result.Files)
            {
                localWebResources.Add(
                    string.IsNullOrEmpty(options.Prefix) ? file.Path.Replace("\\", "/") : Path.Combine(options.Prefix, file.Path).Replace("\\", "/"),
                    Path.Combine(options.SourceFolder, file.Path));
            }
        }

        public void SaveWebResourcesInMemory()
        {
            memoryWebResources.Clear();
            foreach (var webResource in webResources)
            {
                var byteContent = webResource.Content!=null ? Convert.FromBase64String(webResource.Content) : new byte[] { };
                var stream = memoryStreamManager.GetStream(byteContent);
                stream.Seek(0, SeekOrigin.Begin);
                memoryWebResources.Add(webResource.Name, stream);
            }
        }

        public void SaveWebResourcesInTempFolder()
        {
            tempWebResources.Clear();
            var tempFolder = Path.GetTempFileName();
            File.Delete(tempFolder);
            foreach (var webResource in webResources)
            {
                var file = new FileInfo(Path.Combine(tempFolder, webResource.Name));
                file.Directory.Create();
                using (var writer = file.Create())
                {
                    var byteContent = Convert.FromBase64String(webResource.Content);
                    writer.Write(byteContent, 0, byteContent.Length);
                }
                tempWebResources.Add(webResource.Name, file.FullName);
            }
        }
    }
}
