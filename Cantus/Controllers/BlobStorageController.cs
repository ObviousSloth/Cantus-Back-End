using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Cantus.Services;
using Auth0.ManagementApi.Models;

namespace Cantus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobStorageController : ControllerBase
    {
        private readonly FileService _fileservice;
        public BlobStorageController(FileService fileService) 
        {
            _fileservice = fileService;
        }


        [HttpGet]
        public async Task<IActionResult> ListAllBlobs()
        {
            try
            {
                var result = await _fileservice.ListAsync();
                return Ok(result);
            }
            catch (Exception EX)
            {

                return BadRequest(EX.Message);
            }
          


        }
        [HttpPost]
        [Route("Upload-music")]
        public async Task<IActionResult> UploadMusic(IFormFile music)
        {
            try
            {
                var results = await _fileservice.UploadMusicAsync(music);
                return Ok(results);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Route ("Upload-file")]
        public async Task<IActionResult> UploadFiles(IFormFile file)
        {
            try
            {
                var results = await _fileservice.UploadFilesAsync(file);
                return Ok(results);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Route("Download")]
        public async Task<IActionResult> DownloadFiles(string FileName)
        {
            try
            {
                var results = await _fileservice.DownloadAsync(FileName);
                return File(results.Content, results.ContentType, results.Name);
            }
            catch (Exception ex)
            {

                return NotFound(ex.Message);
            }
            
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string FileName)
        {
            try
            {
                var result = await _fileservice.DeleteAsync(FileName);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

    }
}
