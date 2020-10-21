using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        public ActivityController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(DAL.Activity.Instance.GetCount());
        }
        [HttpGet("verifyCount")]
        public ActionResult GetVerifyCount()
        {
            return Ok(DAL.Activity.Instance.GetVerifyCount());
        }
        [HttpGet("recommend")]
        public ActionResult GetRecommend()
        {
            var result = DAL.Activity.Instance.GetRecommend();
            if (result != null)
                return Ok(Result.Ok(result));
            else
                return Ok(Result.Err("��¼��Ϊ0"));
        }
        [HttpGet("end")]
        public ActionResult GetEnd()
        {
            var result = DAL.Activity.Instance.GetEnd();
            if (result != null)
            {
                return Ok(Result.Ok(result));
            }
            else
                return Ok(Result.Err("û���κλ"));
        }
        [HttpGet("names")]
        public ActionResult GetNames()
        {
            var result = DAL.Activity.Instance.GetActivityNames();
            if (result.Count() == 0)
            {
                return Ok(Result.Err("û���κλ"));
            }
            else
                return Ok(Result.Ok(result));
        }
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var result = DAL.Activity.Instance.GetModel(id);
            result.activityIntroduction = result.activityIntroduction.Replace("src=\"", $"src=\"https://{HttpContext.Request.Host.Value}/");
            if (result != null)
            {
                return Ok(Result.Ok(result));
            }
            else
                return Ok(Result.Err("activityID������"));

        }
        [HttpPost]
        public ActionResult Post([FromBody]Model.Activity activity)
        {
            activity.activityIntroduction = activity.activityIntroduction.Replace($"https://{HttpContext.Request.Host.Value}/", "");
            activity.recommend = "��";
            try
            {
                int n = DAL.Activity.Instance.Add(activity);
                return Ok(Result.Ok("������ɹ�", n));
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("foreign key"))
                    return Ok(Result.Err("�Ϸ��û�������Ӽ�¼"));
                else if (ex.Message.ToLower().Contains("null"))
                    return Ok(Result.Err("����ơ�����ʱ�䡢�ͼƬ������������û�������Ϊ��"));
                else
                    return Ok(Result.Err(ex.Message));

            }
        }
        [HttpPut]
        public ActionResult Put([FromBody]Model.Activity activity)
        {
            activity.activityIntroduction = activity.activityIntroduction.Replace($"https://{HttpContext.Request.Host.Value}/", "");
            try
            {
                var n = DAL.Activity.Instance.Update(activity);
                if (n != 0)
                    return Ok(Result.Ok("�޸Ļ�ɹ�", activity.activityId));
                else
                    return Ok(Result.Err("activityID������"));
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("null"))
                    return Ok(Result.Err("����ơ�����ʱ�䡢�ͼƬ�������������Ϊ��"));
                else
                    return Ok(Result.Err(ex.Message));
            }
        }
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var n = DAL.Activity.Instance.Delete(id);
                if (n != 0)
                    return Ok(Result.Ok("ɾ���ɹ�"));
                else
                    return Ok(Result.Err("activityID������"));
            }
            catch (Exception ex)
            {
                return Ok(Result.Err(ex.Message));
            }
        }
        [HttpPost("page")]
        public ActionResult getPage([FromBody]Model.Page page)
        {
            var result = DAL.Activity.Instance.GetPage(page);
            if (result.Count() == 0)
                return Ok(Result.Err("���ؼ�¼��Ϊ0"));
            else
                return Ok(Result.Ok(result));
        }
        [HttpPost("verifyPage")]
        public ActionResult getVerifyPage([FromBody]Model.Page page)
        {
            var result = DAL.Activity.Instance.GetVerifyPage(page);
            if (result.Count() == 0)
                return Ok(Result.Err("���ؼ�¼��Ϊ0"));
            else
                return Ok(Result.Ok(result));
        }
        [HttpPut("Verify")]
        public ActionResult PutVerify([FromBody]Model.Activity activity)
        {
            try
            {
                var n = DAL.Activity.Instance.UpdateVerify(activity);
                if (n != 0)
                    return Ok(Result.Ok("��˻�ɹ�", activity.activityId));
                else
                    return Ok(Result.Err("activityId������"));
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("null"))
                    return Ok(Result.Err("�����������Ϊ��"));
                else
                    return Ok(Result.Err(ex.Message));
            }
        }
        [HttpPut("recommend")]
        public ActionResult PutRecommend([FromBody] Model.Activity activity)
        {
            activity.recommendTime = DateTime.Now;
            try
            {
                var re = "";
                if (activity.recommend == "��") re = "ȡ��";
                var n = DAL.Activity.Instance.UpdateRecommend(activity);
                if (n != 0)
                {
                    return Ok(Result.Ok($"{re}�Ƽ���ɹ�", activity.activityId));
                }
                else
                    return Ok(Result.Err("activityId������"));
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("null"))
                    return Ok(Result.Err("�Ƽ���������Ϊ��"));
                else
                    return Ok(Result.Err(ex.Message));
            }

        }
        [HttpPut("{id}")]
        public ActionResult upImg(int id ,List<IFormFile> files) 
        {
            var path = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "img", "Activity");
            var fileName = $"{path}/{id}";
            try 
            {
                var ext = DAL.Upload.Instance.UpImg(files[0], fileName);
                if (ext == null)
                    return Ok(Result.Err("���ϴ�ͼƬ�ļ�"));
                else 
                {
                    var file = $"img/Activity/{id}/{ext}";
                    Model.Activity activity = new Model.Activity()
                    {
                        activityId = id,
                        activityPicture = file
                    };
                    var n = DAL.Activity.Instance.UpdateImg(activity);
                    if (n > 0)
                        return Ok(Result.Ok("�ϴ��ɹ�", file));
                    else
                        return Ok(Result.Err("��������ȷ�Ļid"));
                }

            }
            catch (Exception ex)
            {
                return Ok(Result.Err(ex.Message));
            }
        }
    }
}