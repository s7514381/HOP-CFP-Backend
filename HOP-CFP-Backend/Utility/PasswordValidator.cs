using System.Text.RegularExpressions;

namespace HOP_CFP_Backend.Utility
{
    public class PasswordValidator
    {
        /// <summary>
        /// 最少需要符合的驗證種類數
        /// </summary>
        private int _minValidCount = 3;
        /// <summary>
        /// 最少需要符合的驗證種類數
        /// </summary>
        public int MinValidCount { get; set; }

        private int _minLength = 8;
        public int MinLength
        {
            get
            {
                return _minLength;
            }
            set
            {
                _minLength = value;
            }
        }

        private int _maxLength = 32;
        public int MaxLength
        {
            get
            {
                return _maxLength;
            }
            set
            {
                _maxLength = value;
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        public PasswordValidator() { }

        public bool Valid(out string message)
        {
            //驗證長度
            if (false == ValidLength(out message))
                return false;

            int ValidTypeCount = 0;
            if (IncludeNumber())
                ValidTypeCount++;

            if (IncludeUppercaseCharacter())
                ValidTypeCount++;

            if (IncludeLowercaseCharacter())
                ValidTypeCount++;

            if (IncludeSpecialCharacter())
                ValidTypeCount++;

            if (ValidTypeCount < _minValidCount)
            {
                message = "密碼至少需包含特殊符號、大寫英文、小寫英文、數字其中3種";
                return false;
            }
            else
            {
                message = "驗證成功";
                return true;
            }
        }

        public bool ValidLength(out string message)
        {
            if (_password.Length < _minLength || _password.Length > _maxLength)
            {
                message = $"密碼長度需在{_minLength}至{_maxLength}個字元";
                return false;
            }
            else
            {
                message = "驗證成功";
                return true;
            }
        }

        public bool IncludeNumber()
        {
            Regex r = new Regex("\\d");
            return r.Match(_password).Success;
        }

        public bool IncludeUppercaseCharacter()
        {
            Regex r = new Regex("[A-Z]");
            return r.Match(_password).Success;
        }

        public bool IncludeLowercaseCharacter()
        {
            Regex r = new Regex("[a-z]");
            return r.Match(_password).Success;
        }

        public bool IncludeSpecialCharacter()
        {
            Regex r = new Regex("[*.!@#$%^&(){}\\[\\]:\"; '<>,.?/~`_+\\-=|\\\\]");
            return r.Match(_password).Success;
        }
    }
}