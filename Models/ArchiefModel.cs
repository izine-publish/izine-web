//---------------------------------------------------------------------------------------------------------------
//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Models/ArchiefModel.cs $

//    Owner: Prakash Bhatt

//    $Date: 2010-01-28 17:16:16 +0530 (Thu, 28 Jan 2010) $

//    $Revision: 770 $

//    $Author: prakash.bhatt $

//    Description: Model class for Archief

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iZINE.Web.MVC.Models
{
    public class Result
    {
        private String _id;
        private string _author;
        private string _subject;
        private string _title;
        private string _pdfFile;
        private DateTime _publicationDate;
        private string _publicationDateString;

        public String Id
        {
            get
            {
                return (_id);
            }
            set
            {
                _id = value;
            }
        }

        public String Author
        {
            get
            {
                return (_author);
            }
            set
            {
                _author = value;
            }
        }


        public String Title
        {
            get
            {
                return (_title);
            }
            set
            {
                _title = value;
            }
        }


        public String Subject
        {
            get
            {
                return (_subject);
            }
            set
            {
                _subject = value;
            }
        }

        public DateTime PublicationDate
        {
            get
            {
                return (_publicationDate);
            }
            set
            {
                _publicationDate = value;
            }
        }

        public String PdfFile
        {
            get
            {
                return (_pdfFile);
            }
            set
            {
                _pdfFile = value;
            }
        }
        public String PublicationDateString
        {
            get
            {
                return (_publicationDateString);
            }
            set
            {
                _publicationDateString = value;
            }
        }
    }
}
