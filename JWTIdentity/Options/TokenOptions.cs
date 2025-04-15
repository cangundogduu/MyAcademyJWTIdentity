namespace JWTIdentity.Options
{
    public class TokenOptions
    {
        public string Issuer { get; set; }//yayınlayan
        public string Audience { get; set; }//hedef-dinleyen
        public string Key { get; set; }
        public int ExpireInMinutes{ get; set; } //token süresi


    }
}
