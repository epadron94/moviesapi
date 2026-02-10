namespace moviesapi.Utilities;
using System;
using System.Text;

public class Utilities
{
    public Func<string, string> Encode = token => string.IsNullOrEmpty(token) ? null : Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));
    public Func<string, string> Decode = token => string.IsNullOrEmpty(token) ? null : System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));
}