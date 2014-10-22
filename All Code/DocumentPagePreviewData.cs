//---------------------------------------------------------------------------------------------------------------
//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/All Code/DocumentPagePreviewData.cs $

//   Owner: Prakash Bhatt

//    $Date: 2010-07-01 15:21:26 +0530 (Thu, 01 Jul 2010) $

//    $Revision: 1593 $

//    $Author: prakash.bhatt $

//    Description: class for document page preview

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iZINE.Businesslayer;

/// <summary>
/// Summary description for DocumentPagePreviewData
/// </summary>
public class DocumentPagePreviewData
{
    public DocumentPagePreviewData()
    {        
    }

    public int? Number { get; set; }
    public Constant Type { get; set; }
    public Guid PageId { get; set; }
    public Guid AssetId { get; set; }
    public Guid VersionId { get; set; }
    public string Name { get; set; }
    public Guid StatusId { get; set; }
    public bool IsBlank { get; set; }
	public String StatusImage { get; set; }
	public Guid StatusState { get; set; }
}

public class DocumentGroupPage
{
    public DocumentPagePreviewData First { get; set; }
    public DocumentPagePreviewData Second { get; set; }

}
