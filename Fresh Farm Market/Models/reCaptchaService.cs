using Newtonsoft.Json;

namespace Fresh_Farm_Market.Models
{
	public class reCaptchaService
	{
        public virtual async Task<reCaptchaRespo> tokenVerify(string token)
        {
            reCaptchaData data = new reCaptchaData
            {
                response = token,
                secret = "6LdZTQAkAAAAAINj3kH7a9sMELt4aBkom56S1-va"
            };

            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={data.secret}&response={data.response}");
            var reCaptcharesponse = JsonConvert.DeserializeObject<reCaptchaRespo>(response);
            return reCaptcharesponse;
        }
    }
    public class reCaptchaData
    {
        public string response { get; set; }
        public string secret { get; set; }
    }
    public class reCaptchaRespo
    {
        public bool success { get; set; }
        public DateTime challenge_ts { get; set; }
        public string hostname { get; set; }
        public long score { get; set; }
    }
}
