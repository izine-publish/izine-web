using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

public class IZineWebConfiguration:ConfigurationSection
{
    public IZineWebConfiguration()
    {
        
    }

    private static IZineWebConfiguration _section = null;

    public static IZineWebConfiguration GetSection()
    {
        if (_section == null)
        {
            _section = ConfigurationManager.GetSection("iZineWeb") as IZineWebConfiguration;
        }

        return _section;
    }

    [ConfigurationProperty("version", DefaultValue="")]
    public string Version
    {
        get
        {            
            return (string)base["version"];
        }
    }
}
