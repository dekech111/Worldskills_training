using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApplication2.Controllers
{
    public class DefaultController : ApiController
    {
        [AcceptVerbs("GET")]
        [Route("Ping/{id}")]
        public async Task<IHttpActionResult> Ping(int id)
        {
            return Ok(new {Id = id, Имя = "Илья", Фамилия = "Шадрин"});
        }

        [AcceptVerbs("POST")]
        [Route("PostData")]
        public async Task<IHttpActionResult> PostData([FromBody] string[] data)
        {
            return Ok(new { date = DateTime.Now, data });
        }

        [AcceptVerbs("POST")]
        [Route("UploadFiles")]
        public async Task<IHttpActionResult> UploadFiles()
        {
            try
            {
                MultipartMemoryStreamProvider provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                if(provider.Contents != null && provider.Contents.Count > 0)
                {
                    foreach (var cont in provider.Contents)
                    {
                        if (!string.IsNullOrEmpty(cont.Headers.ContentDisposition.FileName))
                        {
                            var filename = cont.Headers.ContentDisposition.FileName;

                            var saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
                            if (!Directory.Exists(saveDir))
                            {
                                Directory.CreateDirectory(saveDir);
                            }

                            var filePath = Path.Combine(saveDir, filename);
                            if(File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }

                            var bites = await cont.ReadAsByteArrayAsync();
                            if(bites != null && bites.Length > 0)
                            {
                                using (var streamMemory = new MemoryStream(bites))
                                {
                                    streamMemory.Seek(0, SeekOrigin.Begin);
                                    using (var fileStream = File.Create(filePath))
                                    {
                                        streamMemory.CopyTo(fileStream);
                                    }
                                }
                            }
                            else { return Ok(new { message = "Файл пустой!" }); }
                        }
                        else { return Ok(new { message = "Файл без имени!" }); }
                    }

                    return Ok(new { message = "Успех" });
                }
                else { return Ok(new { message = "Файл не найден" }); }
            }
            catch (Exception ex) { return InternalServerError(ex); }
        }
    }
}
