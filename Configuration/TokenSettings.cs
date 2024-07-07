namespace DevJobsBackend.Configurations;

public class TokenSettings
    {
        public string SecretToken { get; set; }
        public string RefreshTokenSecret { get; set; }
        public string ForgotPasswordSecret { get; set; }
        public string DeleteAccountTokenSecret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
