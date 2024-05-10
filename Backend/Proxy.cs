using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ItemFinder_WPF.Backend.Enums;

namespace ItemFinder_WPF.Backend
{
    public class Proxy
    {
        private string _username;
        private string _password;
        private string _ip;
        private uint _port;
        private ProxyMethod _proxyMethod;

        public Proxy(string username, string password, string ip, uint port, ProxyMethod proxyMethod)
        {
            _username= username;
            _password= password;
            _ip= ip;
            _port= port;
            _proxyMethod= proxyMethod;
        }

        public IWebProxy GetWebProxy()
        {
            UriBuilder uriBuilder = new UriBuilder()
            {
                Scheme = _proxyMethod.ToString().ToLower(),
                Host = _ip,
                Port = (int)_port
            };

            IWebProxy proxy = new WebProxy(uriBuilder.ToString());
            proxy.Credentials = new NetworkCredential(_username, _password);
            return proxy;
        }
    }
}
