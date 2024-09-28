﻿using System.Net;

namespace RpdPlayerApp.Architecture;

internal class MyClient : WebClient
{
    public bool HeadOnly { get; set; }
    protected override WebRequest GetWebRequest(Uri address)
    {
        WebRequest req = base.GetWebRequest(address);
        if (HeadOnly && req.Method == "GET")
        {
            req.Method = "HEAD";
        }
        return req;
    }
}
