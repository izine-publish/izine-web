
//---------------------------------------------------------------------------------------------------------------
//	$HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Server.cs $

//	Owner: Prakash Bhatt

//	$Date: 2010-10-28 18:26:54 +0530 (Thu, 28 Oct 2010) $

//	$Revision: 2419 $

//	$Author: rajkumar.sehrawat $

//	Description: implementaion for Iserver

//	Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using iZINE.Web.Utils;
using iZINE.Web.Server;
using iZINE.Businesslayer;
using System.Web;
using System.Xml.Linq;
using System.Data.Objects;
using System.Linq.Expressions;
using System.Drawing;
using System.Net.Mail;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.Zip.Compression;
using Amazon.S3;
using Amazon.S3.Model;
using System.Configuration;

public enum enTaskColumns
{
	eDOCUMENTID = 1,
	eASSIGNTO,
	eSTAUSID,
	eCATEGORY,
	eSPREAD,
	ePAGE,
	eDESCRIPTION,
	eSUBJECT,
	eEDITIONID,
	eCOMMENTS

}

// cannot use getbyid etc, because these are using an other datacontext / objectcontext

//TODO: transaction handling.

[ServiceBehavior(	InstanceContextMode = InstanceContextMode.PerCall)]
public class Server : IServer
{
	private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	
	// [PrincipalPermission(SecurityAction.Demand, Role = "Administrators")]
	public Server()
	{
		log.DebugFormat("Server Object created.");
	}

	public TitleDTO[] GetTitleList()
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>();

			iZINE.Businesslayer.User user = context.Users.FirstOrDefault(u => u.Membership.Username.CompareTo(OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name) == 0 && u.Active==true) as User;
			if (user == null)
				throw new Exception("User account not found");

			user.Titles.Load();
			IEnumerable<TitleDTO> titleList = from c in user.Titles
											  where c is Title && c.Active == true
											  orderby c.Name
											  select new TitleDTO
											  {
												  constantid = c.TitleId,
												  name = c.Name
											  };
			return (titleList.ToArray<TitleDTO>());
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing GetTitleList  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	public Guid GetRole()
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>();

			User user = context.Users.Include("Role").FirstOrDefault(u => u.Membership.Username.CompareTo(OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name) == 0 && u.Active == true) as User;
			return (user.Role.RoleId);

		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing GetRole  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	public CommentDTO[] GetCommentList(Guid assetid)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>();

			IEnumerable<CommentDTO> commentList = from c in context.Comments
												  where c.Asset.AssetId == assetid && c.Active == true
												  orderby c.Date descending
												  select new CommentDTO
												  {
													  assetid = c.Asset.AssetId,
													  name = c.Asset.Name,
													  comment = c.Text,
													  statusid = c.Asset.Version.status.StatusId,
													  status = c.Asset.Version.status.Name,
													  userid = c.Asset.Version.User.UserId,
													  username = c.Asset.Version.User.Membership.Username,
													  date = c.Date,
													  number = c.Asset.Version.Number,
													  versionid = c.Asset.Version.VersionId
												  };
			return (commentList.ToArray<CommentDTO>());
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing GetCommentList  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	public String[] GetTags(Guid assetid)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			// should we use assetid or titleid
			return (new String[] { "kop", "chapeau", "intro", "body", "body_opsomming", "body_eerste_alinea", "tussenkop", "streamer", "kaderkop", "kadertekst", "kadertekst_code", "bijschrift", "credit", "noot_kopje", "noot", "auteur", "auteur_uitleg", "rubriek", "body_cursief", "intro_cursief", "body_eerste_alinea", "body_code", "cursief", "bold", "head", "content", "foot", "Afbeelding" });
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing GetTags  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	public void UploadPage(Guid pageid, Guid versionid, int number, Guid pagetypeid, byte[] data, bool isCompressed)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{

				iZINE.Businesslayer.Version version = context.Versions.Include("Asset").FirstOrDefault(v => v.VersionId == versionid) as iZINE.Businesslayer.Version;
				if (version == null)
					throw new Exception("Version not found");

				version.Asset.LockReference.Load();
				version.Asset.Lock.OwnerReference.Load();
				version.Asset.Lock.Owner.MembershipReference.Load();

				iZINE.Businesslayer.User user = context.Users.FirstOrDefault(u => u.Membership.Username.CompareTo(OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name) == 0 && u.Active == true) as User;

				if (!version.Asset.Lock.Owner.Equals(user))
					throw new Exception("Asset is locked by defferent user");


				iZINE.Businesslayer.Constant pageType = context.Constants.FirstOrDefault(c => c.ConstantId.CompareTo(pagetypeid) == 0) as iZINE.Businesslayer.Constant;
				if (pageType == null)
					throw new Exception("pageType not found");

				Page page = Page.CreatePage(pageid);
				page.Version = version;
				page.Type = pageType;
				page.Number = number;

				context.AddToPages(page);
				context.SaveChanges();

				String imagesDir = System.Configuration.ConfigurationManager.AppSettings["imagesDir"];
				version.Asset.ShelveReference.Load();
				version.Asset.TitleReference.Load();
				imagesDir = Path.Combine(imagesDir, String.Format("{0}/{1}/{2}", version.Asset.Title.TitleId, version.Asset.Shelve.Date.Year.ToString(), version.Asset.Shelve.ShelveId.ToString()));
				if (!Directory.Exists(imagesDir))
					Directory.CreateDirectory(imagesDir);

				String imagePath = Path.Combine(imagesDir, String.Format("{0}.jpg", pageid.ToString()));
				System.IO.File.WriteAllBytes(imagePath, data);

				String thumbnailsDir = System.Configuration.ConfigurationManager.AppSettings["thumbnailsDir"];
				thumbnailsDir = Path.Combine(thumbnailsDir, String.Format("{0}/{1}/{2}", version.Asset.Title.TitleId, version.Asset.Shelve.Date.Year.ToString(), version.Asset.Shelve.ShelveId.ToString()));

				if (!Directory.Exists(thumbnailsDir))
					Directory.CreateDirectory(thumbnailsDir);

				String thumbnailPath = Path.Combine(thumbnailsDir, String.Format("{0}.jpg", pageid.ToString()));

				int width = 120;
				int height = 170;

				System.Drawing.Bitmap originalBitmap = new System.Drawing.Bitmap(imagePath);
				System.Drawing.Bitmap thumbnailBitmap = new System.Drawing.Bitmap(width, height);

				decimal lnRatio;

				int lnNewWidth = 0;
				int lnNewHeight = 0;

				if (originalBitmap.Width > originalBitmap.Height)
				{
					lnRatio = (decimal)width / originalBitmap.Width;
					lnNewWidth = width;
					decimal lnTemp = originalBitmap.Height * lnRatio;
					lnNewHeight = (int)lnTemp;
				}
				else
				{
					lnRatio = (decimal)height / originalBitmap.Height;
					lnNewHeight = height;
					decimal lnTemp = originalBitmap.Width * lnRatio;
					lnNewWidth = (int)lnTemp;
				}

				Graphics g = Graphics.FromImage(thumbnailBitmap);
				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				g.FillRectangle(Brushes.White, 0, 0, lnNewWidth, lnNewHeight);
				g.DrawImage(originalBitmap, 0, 0, lnNewWidth, lnNewHeight);
				g.Dispose();

				thumbnailBitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);

				originalBitmap.Dispose();
				thumbnailBitmap.Dispose();
			}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing UploadPage  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	//public AssetDTO[] GetAssetList_Old(Guid shelveid, Guid[] types)
	//{
	//	try {
	//		using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
	//		{
	//			/*
	//			Expression<Func<Asset, bool>> predicate = t => false;

	//			foreach (Guid type in types)
	//			{
	//				predicate = iZINE.Web.Utils.Linq.Utility.Or<Asset>(predicate, p => p.Type.ConstantId.CompareTo(type) == 0);
	//			}
	//			*/

	//			// compiling query for performance
	//			Func<iZINEEntities, Guid, Guid, IQueryable<Asset>> GetAssetsByShelveIdAndTypeId =
	//				CompiledQuery.Compile((iZINEEntities entities, Guid shelveid2, Guid typeid2) =>
	//				(
	//					from a in entities.Assets.Include("Shelve").Include("Title").Include("Type").Include("Version").Include("Lock").Include("Version.Status").Include("Version.User").Include("Version.User.Membership")
	//					where a.Active == true && a.Shelve.ShelveId == shelveid2 && a.Type.ConstantId == typeid2
	//					select a
	//				)
	//			);

	//			List<AssetDTO> assetList=new List<AssetDTO>(); 
				
	//			// it is a damn ugly expression, but it works
	//			foreach (Guid type in types)
	//			{
	//				// Include("Lock") removed, 
	//				IEnumerable<AssetDTO> list= from b in
	//													  (
	//														GetAssetsByShelveIdAndTypeId(context, shelveid, type).AsEnumerable()
	//													  )
	//												  select new AssetDTO
	//												  {
	//													  assetid = b.AssetId,
	//													  name = b.Name,
	//													  documentid = (b.Document != null ? b.Document.AssetId : (Guid?)null),
	//													  documentname = (b.Document != null ? b.Document.Name : null),
	//													  titleid = (b.Title != null ? b.Title.TitleId : (Guid?)null),
	//													  shelveid = (b.Shelve != null ? b.Shelve.ShelveId : (Guid?)null),
	//													  type = (b.Type != null ? b.Type.Name : null),
	//													  typeid = (b.Type != null ? b.Type.ConstantId : (Guid?)null),
														  
	//													  Head = new VersionInfo
	//																{
	//																	versionid = (b.Version != null ? b.Version.VersionId : (Guid?)null),
	//																	number = (b.Version != null ? b.Version.Number : null),
	//																	date = (b.Version != null ? b.Version.Date.ToUniversalTime() : (DateTime?)null),
	//																	user = (b.Version != null && b.Version.User != null && b.Version.User.Membership != null
	//																	  ? new UserDTO
	//																	  {
	//																		  userid = b.Version.User.UserId,
	//																		  name = b.Version.User.Fullname,
	//																		  username = b.Version.User.Membership.Username,
	//																	  }
	//																	  : null),
	//																	statusid = (b.Version != null && b.Version.status != null ? b.Version.status.StatusId : (Guid?)null),
	//													  },
	//													  lockid = (b.Lock != null ? b.Lock.LockId : (Guid?)null),
														  

	//												  };

	//				assetList.AddRange(list.ToArray<AssetDTO>());
	//			}

	//			return (from a 
	//						in assetList 
	//					orderby a.date descending
	//					select a)
	//					.ToArray<AssetDTO>();
	//		}
	//	}
	//	catch (Exception exc)
	//	{
	//		log.Error(exc);
	//		throw;
	//	}
	//}

	public ShelveDTO[] GetShelveList(Guid titleid)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try {
		iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>();

		IEnumerable<ShelveDTO> shelvesList = from s in context.Shelves
											 where s.Title.TitleId == titleid && s.Active == true
											 orderby s.Name
											 select new ShelveDTO
											 {
												 shelveid = s.ShelveId,
												 name = s.Name
											 };

		return (shelvesList.ToArray<ShelveDTO>());
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing GetShelveList  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	public bool Logoff()
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		log.DebugFormat("User '{0}' logged off.", OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name);
		int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		log.InfoFormat("finished processing Logoff  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		
		return (true);
	}

	public UserDTO Login()
	{
		log.DebugFormat("User '{0}' logged in.", OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name);
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{


				UserDTO userdto = (from u in
									   (
									   from u1 in context.Users.Include("Membership")
									   where u1.Membership.Username == OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name && u1.Active == true
									   select u1).AsEnumerable()
								   select new UserDTO
								   {
									   userid = u.UserId,
									   username = u.Membership.Username,
									   name = u.Fullname
								   }).FirstOrDefault();
				return (userdto);
			}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing login  in {0} seconds",(iTotalEndTime - iTotalStartTime));
		}
	}

	public UserDTO GetUser()
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try {
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{


				UserDTO userdto = (from u in
										(
										from u1 in context.Users.Include("Membership")
										where u1.Membership.Username == OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name && u1.Active == true
										select u1).AsEnumerable()
									select new UserDTO
									{
										userid = u.UserId,
										username = u.Membership.Username,
										name = u.Fullname
									}).FirstOrDefault();
				return (userdto);
			}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing GetUser  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	public byte[] GetThumbnail(Guid assetid)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try {
		// use versionid instead of assetid
		iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>();
		
		Asset asset = context.Assets.FirstOrDefault<Asset>(a => a.AssetId == assetid && a.Active == true);
		if (asset == null)
			return (null);

		// get thumbnail for associated document
		Asset document = null;

		asset.DocumentReference.Load();

		asset.TypeReference.Load();

		if (asset.Type.ConstantId == new Guid("a3871d22-7d8b-4d9f-901e-4c284b1a801e"))
		{
			document = asset;
		}
		else if (asset.Type.ConstantId == new Guid("BDEC0BF8-9591-4533-AFEA-467D0B340E92"))
		{
			document = asset.Document;
		}
		else if (asset.Type.ConstantId == new Guid("756779a5-bb56-4430-9e95-022226970095"))
		{
			document = asset.Document;
		}
		else if (asset.Type.ConstantId == new Guid("0DB0B0BC-25A8-4B5E-81B1-819336832D68"))
		{
			document = asset.Document;
		}
		else if (asset.Type.ConstantId == new Guid("2326e21b-f4fb-40eb-acf6-46454d4d9f9f"))
		{
			document = asset;
		}
		else if (asset.Type.ConstantId == new Guid("b4eee6f9-6061-481e-8a6e-f89dfbebad4e"))
		{
			document = asset.Document;
		}

		if (document == null)
			return (null);

		var page=			(from d
								in context.Assets
								from p in d.Version.Pages
								orderby p.Number
								where d.AssetId==document.AssetId
								select p
							).FirstOrDefault();

		if (page == null)
			return (null);

		asset.TitleReference.Load();
		asset.ShelveReference.Load();
		String thumbnailsDir = System.Configuration.ConfigurationManager.AppSettings["thumbnailsDir"];
		thumbnailsDir = Path.Combine(thumbnailsDir, String.Format("{0}/{1}/{2}", asset.Title.TitleId,asset.Shelve.Date.Year.ToString(),asset.Shelve.ShelveId));

		String thumbnailPath = Path.Combine(thumbnailsDir, String.Format("{0}.jpg", page.PageId.ToString()));
		if (File.Exists(thumbnailPath))
		{
			return File.ReadAllBytes(thumbnailPath);
		}

		return (null);
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing GetThumbnail  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	public LockDTO LockAsset(Guid assetid, LockDTO lockDTO)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try {
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{
				LockDTO lockDTOResponse = new LockDTO();
				Asset asset = context.Assets.Include("Lock").FirstOrDefault<Asset>(a => a.AssetId == assetid);
				if (asset == null)
					return lockDTOResponse;



				if (asset.Lock == null)
				{


					Lock lockInfo = new Lock();
					lockInfo.LockId = Guid.NewGuid();
					lockInfo.ApplicationName = lockDTO.applicationname;
					lockInfo.DocumentName = lockDTO.documentname;
					lockInfo.Date = DateTime.Now;
					lockInfo.Owner = context.Users.FirstOrDefault(u => u.Membership.Username.CompareTo(OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name) == 0 && u.Active == true) as User;
					lockInfo.Asset = asset;
					asset.Lock = lockInfo;
					context.AddToLocks(lockInfo);

					context.SaveChanges();
				}
				else
				{
					asset.Lock.OwnerReference.Load();
					asset.Lock.Owner.MembershipReference.Load();
				}
				
			  

				lockDTOResponse.applicationname = asset.Lock.ApplicationName;
				lockDTOResponse.assetid = asset.Lock.Asset.AssetId;
				lockDTOResponse.documentname = lockDTO.documentname;
				lockDTOResponse.lockid = asset.Lock.LockId;
				lockDTOResponse.userid = asset.Lock.Owner.UserId;
				lockDTOResponse.username = asset.Lock.Owner.Fullname;

				return lockDTOResponse;
			}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing LockAsset  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	public bool UnlockAsset(Guid assetid,bool forceUnlock)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try {

			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{

				Asset asset = context.Assets.FirstOrDefault<Asset>(a => a.AssetId == assetid && a.Active == true);
				if (asset == null)
					throw new Exception("Asset not found");

				asset.LockReference.Load();
				if (asset.Lock == null)
				{
					return true;
				}
				asset.Lock.OwnerReference.Load();
				asset.Lock.Owner.MembershipReference.Load();
				User user = context.Users.FirstOrDefault(
					u => u.Membership.Username.CompareTo(
						OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name) 
						== 0 && u.Active == true) as User;
				// only unlock by owner or admin!
				user.RoleReference.Load();
				
				if (asset.Lock.Owner.UserId == user.UserId)
				{
					asset.Lock = null;
				}
				else if (asset.Lock != null && user.Role.RoleId.Equals(new Guid("0d629cba-f55c-461a-831e-58053db7189f")))
				{
					asset.Lock = null;
				}
				else
				{
					return false;
				}
				context.SaveChanges();
			}
			return true;
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing UnLockAsset  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	

	public void DeleteAsset(Guid assetid)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try {
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{

				Asset asset = context.Assets.FirstOrDefault<Asset>(a => a.AssetId == assetid);
				if (asset == null)
					throw new Exception("asset not found");
				asset.LockReference.Load();

				if(asset.Lock != null)
					throw new Exception("asset is locked");

				asset.Active = false;

				asset.Assets.Load();
				foreach (Asset child in asset.Assets)
				{
					child.Active = false;
				}

				context.SaveChanges();
			}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing DeleteAsset  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	public AssetDTO CreateAsset(AssetDTO assetDTO)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>();

			Shelve shelve = context.Shelves.Include("Title").FirstOrDefault(s => s.ShelveId == assetDTO.shelveid);
			if (assetDTO.shelveid != Guid.Empty && shelve == null)
				throw new Exception("shelve not found");

			Asset document = context.Assets.FirstOrDefault(d => d.AssetId == assetDTO.documentid);

			iZINE.Businesslayer.Type type = context.Constants.FirstOrDefault(t => t.ConstantId == assetDTO.typeid) as iZINE.Businesslayer.Type;
			if (type == null)
				throw new Exception("type not found");

			Asset asset = new Asset();
			asset.AssetId = (Guid.Empty == assetDTO.assetid) ? Guid.NewGuid() : assetDTO.assetid;
			
			asset.Name = assetDTO.name;
			asset.Document = document;
			asset.Active = true;
			asset.Date = DateTime.Now;
			
			if (shelve != null)
			{
				asset.Shelve = shelve;
				asset.Title = shelve.Title;
			}

			asset.Type = type;
			asset.Lock = null;

			context.SaveChanges();
			asset.TypeReference.Load();
			asset.ShelveReference.Load();
			asset.TitleReference.Load();

			return new AssetDTO{
				assetid = asset.AssetId,documentid=asset.Document == null ? 
						(Guid?)null :asset.Document.AssetId,documentname =asset.Document == null ? 
						null :asset.Document.Name,Head =null,lockid = (Guid?)null,
						name = asset.Name,shelveid = asset.Shelve.ShelveId,
						titleid = asset.Title.TitleId,type = asset.Type.Name,typeid = asset.Type.ConstantId };
			
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing CreateAsset  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	
	public void UpdateAssetName(Guid assetid, String name)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try {
		iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>();
		
		Asset asset = (from a in context.Assets
						where a.AssetId == assetid
						select a).FirstOrDefault();

		if (asset == null)
			throw new Exception("asset not found");

		asset.Name = name;

		context.SaveChanges();
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing UpdateAssetName  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	public AssetDTO[] GetAsset(Guid[] assetids)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{

				// compiling query for performance
				Func<iZINEEntities, Guid, IQueryable<Asset>> GetAssetsById =
					CompiledQuery.Compile((iZINEEntities entities, Guid assetid2) =>
					(
						from a in context.Assets.Include("Document").Include("Shelve").Include("Title").Include("Type").Include("Version").Include("Version.Status").Include("Version.User").Include("Version.User.Membership")
						where a.AssetId == assetid2
						select a
					)
				);
				List<AssetDTO> assetList = new List<AssetDTO>();
				foreach (Guid assetid in assetids)
				{


					// it is a damn ugly expression, but it works
					AssetDTO assetDTO = (from b in
											 GetAssetsById(context, assetid).AsEnumerable()
										 select new AssetDTO
										 {
											 assetid = b.AssetId,
											 name = b.Name,
											 documentid = (b.Document != null ? b.Document.AssetId : (Guid?)null),
											 documentname = (b.Document != null ? b.Document.Name : null),
											 titleid = (b.Title != null ? b.Title.TitleId : (Guid?)null),
											 shelveid = (b.Shelve != null ? b.Shelve.ShelveId : (Guid?)null),
											 type = (b.Type != null ? b.Type.Name : null),
											 typeid = (b.Type != null ? b.Type.ConstantId : (Guid?)null),
											 lockid = (b.Lock != null ? b.Lock.LockId : (Guid?)null),

											 Head = new VersionInfo
											 {
												 versionid = (b.Version != null ? b.Version.VersionId : (Guid?)null),
												 number = (b.Version != null ? b.Version.Number : null),
												 date = (b.Version != null ? (DateTime)b.Version.Date.ToUniversalTime() : (DateTime?)null),
												 statusid = (b.Version != null && b.Version.status != null ? b.Version.status.StatusId : (Guid?)null),

												 user = (b.Version != null && b.Version.User != null && b.Version.User.Membership != null
															 ? new UserDTO
															 {
																 userid = b.Version.User.UserId,
																 name = b.Version.User.Fullname,
																 username = b.Version.User.Membership.Username,
															 }
																 : null
													 )
											 }

										 }).FirstOrDefault();

					assetList.Add(assetDTO);
				}
				return (from a
									in assetList
						orderby a.Head.date descending
						select a)
								.ToArray<AssetDTO>();
			}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing GetAsset  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	public void UploadPDF(Guid assetid, Guid versionid, byte[] data)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{

				iZINE.Businesslayer.Version version = context.Versions.FirstOrDefault<iZINE.Businesslayer.Version>(v => v.VersionId == versionid) as iZINE.Businesslayer.Version;

				if (version == null)
					throw new Exception("version not found");

				//version.Asset.LockReference.Load();
				//version.Asset.Lock.OwnerReference.Load();
				//version.Asset.Lock.Owner.MembershipReference.Load();

				//iZINE.Businesslayer.User user = context.Users.FirstOrDefault(u => u.Membership.Username.CompareTo(OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name) == 0 && u.Active == true) as User;

				//if (!version.Asset.Lock.Owner.Equals(user))
				//    throw new Exception("Asset is locked by defferent user");


				String pdfDir = System.Configuration.ConfigurationManager.AppSettings["pdfDir"];

				version.AssetReference.Load();

				version.Asset.ShelveReference.Load();
				version.Asset.TitleReference.Load();
				pdfDir = Path.Combine(pdfDir, String.Format("{0}/{1}/{2}", version.Asset.Title.TitleId, version.Asset.Shelve.Date.Year.ToString(), version.Asset.Shelve.ShelveId.ToString()));

				if (!Directory.Exists(pdfDir))
					Directory.CreateDirectory(pdfDir);

				System.IO.File.WriteAllBytes(Path.Combine(pdfDir, String.Format("{0}.pdf", versionid.ToString())), data);
			}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing UploadPdf  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	public void UploadVersion(Guid assetid, VersionDTO versionDto)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try {
		using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
		{
			Asset asset = context.Assets.Include("Title").FirstOrDefault<Asset>(a => a.AssetId == assetid);
			if (asset == null)
				return;

			iZINE.Businesslayer.User user = context.Users.FirstOrDefault(u => u.Membership.Username.CompareTo(OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name) == 0 && u.Active == true) as User;

			asset.VersionReference.Load();

			//if asset has head version then check its locked by same user 
			if (asset.Version != null)
			{
			
				asset.LockReference.Load();
				if(asset.Lock == null)
					throw new Exception("Asset is not being edited ");

				asset.Lock.OwnerReference.Load();
				asset.Lock.Owner.MembershipReference.Load();

				
				if (!asset.Lock.Owner.Equals(user))
					throw new Exception("Asset is locked by defferent user");
			}
			

			// generate hash of data
			System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create();
			byte[] hashBytes = sha.ComputeHash(versionDto.data);

			StringBuilder sb = new StringBuilder();
			foreach (byte b in hashBytes)
			{
				sb.AppendFormat("{0:x2}", b);
			}

			String hash = sb.ToString();

			iZINE.Businesslayer.Version version = iZINE.Businesslayer.Version.CreateVersion(versionDto.versionId.Value, DateTime.Now);
			log.DebugFormat("new version: {0}", version.VersionId.ToString());

			//zero means not checked-in yet(never made head)
			version.Number = 0;
			
			version.Active = true;
			version.Date = DateTime.Now;
			version.Description = "";
			version.Hash = hash;
			version.Asset = asset;
			version.User = user;
			//asset.Version = version;
			//asset.Versions.Add(version);
			context.AddToVersions(version);

			context.SaveChanges();
			log.DebugFormat("new version: {0} saved", version.VersionId.ToString());

			asset.ShelveReference.Load();
			asset.TitleReference.Load();

			String assetsDir = System.Configuration.ConfigurationManager.AppSettings["assetsDir"];
			assetsDir = Path.Combine(assetsDir, String.Format("{0}/{1}/{2}", asset.Title.TitleId, asset.Shelve.Date.Year.ToString(),asset.Shelve.ShelveId.ToString()));

			if(!Directory.Exists(assetsDir))
				Directory.CreateDirectory(assetsDir);

			String path=null;

			asset.TypeReference.Load();
			if (asset.Type.ConstantId == new Guid("a3871d22-7d8b-4d9f-901e-4c284b1a801e"))
			{
				// save file with versionid!
				path=Path.Combine(assetsDir, String.Format("{0}.indd", version.VersionId.ToString()));
			}
			else if (asset.Type.ConstantId == new Guid("BDEC0BF8-9591-4533-AFEA-467D0B340E92"))
			{
				// save file with versionid!
				path=Path.Combine(assetsDir, String.Format("{0}.incx", version.VersionId.ToString()));
			}
			else if (asset.Type.ConstantId == new Guid("756779a5-bb56-4430-9e95-022226970095"))
			{
				// save file with versionid!
				path=Path.Combine(assetsDir, String.Format("{0}.incx", version.VersionId.ToString()));
			}
			else if (asset.Type.ConstantId == new Guid("0DB0B0BC-25A8-4B5E-81B1-819336832D68"))
			{
				// save file with versionid!
				path=Path.Combine(assetsDir, String.Format("{0}.inct", version.VersionId.ToString()));
			}
			else if (asset.Type.ConstantId == new Guid("2326e21b-f4fb-40eb-acf6-46454d4d9f9f"))
			{
				// save file with versionid!
				path=Path.Combine(assetsDir, String.Format("{0}.indt", version.VersionId.ToString()));
			}
			else if (asset.Type.ConstantId == new Guid("b4eee6f9-6061-481e-8a6e-f89dfbebad4e"))
			{
				// save file with versionid!
				path=Path.Combine(assetsDir, String.Format("{0}.inca", version.VersionId.ToString()));
			}

			if (versionDto.data[0] == 0x1f && versionDto.data[1] == 0x8b)
			{
				// gzip encoding
				Stream ins = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(new MemoryStream(versionDto.data));
				// Stream ins = (new MemoryStream(data));


				FileStream outs = File.Create(path);

				const int BUFFER_LEN = 8 * 1024; // 8K
				byte[] buffer = new byte[BUFFER_LEN];

				int bytesRead = ins.Read(buffer, 0, BUFFER_LEN);
				for (; bytesRead != 0; bytesRead = ins.Read(buffer, 0, BUFFER_LEN))
				{
					outs.Write(buffer, 0, bytesRead);
				}

				outs.Flush();

				ins.Close();
				outs.Close();
			}
			else
			{
				System.IO.File.WriteAllBytes(path, versionDto.data);
			}

			iZINE.Businesslayer.indexqueue queue = iZINE.Businesslayer.indexqueue.Createindexqueue(Guid.NewGuid());
			queue.versions = version;
			context.SaveChanges();

			return;
		}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing uploadVersion  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	
	public void UnlinkAsset(Guid assetid)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{

				Asset asset = context.Assets.FirstOrDefault<Asset>(a => a.AssetId == assetid);
				if (asset == null)
					return;

				asset.Document = null;
				context.SaveChanges();
			}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing UnLinkAsset  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	
	// links the asset to their corresponding document (called after save document)
	public void RelinkAsset(Guid documentid, Guid assetid)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{

				Asset document = context.Assets.FirstOrDefault<Asset>(a => a.AssetId == documentid);
				if (document == null)
					return;

				Asset asset = context.Assets.FirstOrDefault<Asset>(a => a.AssetId == assetid);
				if (asset == null)
					return;

				asset.Document = document;
				context.SaveChanges();
			}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing RelinkAsset  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}
	
	public void CheckOutAsset(string assetid)
	{
	}

	public TaskDTO CreateTask(TaskDTO task)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		tasks objTask = null;
		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{
				User user = context.Users.FirstOrDefault(u => u.Membership.Username.CompareTo(OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name) == 0 && u.Active == true) as User;

				taskcategories category = context.taskcategories.FirstOrDefault(c => c.id== task.categoryId);
				
				objTask = new tasks();
				if (task.assetid.HasValue)
					objTask.assets = context.Assets.Where(a => a.AssetId == task.assetid.Value).FirstOrDefault();
				objTask.CreatedByUser = user;
				if(task.assignToUserId.HasValue)
					objTask.AssignToUser = context.Users.FirstOrDefault(u => u.UserId == task.assignToUserId.Value);
				objTask.taskstatuses = context.taskstatuses.FirstOrDefault(u => u.taskstatusid == task.taskStatusId);
				objTask.taskcategories = category;
				objTask.spread = task.spread;
				objTask.page = task.page;
				objTask.description = task.description;
				objTask.subject = task.subject;
				objTask.lastupdatedatetime = DateTime.Now;
				objTask.LastUpdatedUser = user;
				objTask.creationdatetime = DateTime.Now;
				objTask.taskid = Guid.NewGuid();
				objTask.shelves = context.Shelves.FirstOrDefault(u => u.ShelveId == task.editionId);
				objTask.versionnumber = 0;
				context.SaveChanges();
			}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing CreateTask  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}

		return (TaskDTO)objTask;
	}

	public TaskDTO[] GetEditionTaskList(Guid editionId)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		List<TaskDTO> tasks = new List<TaskDTO>();
		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{
				tasks = (from i in context.tasks.Include("AssignToUser").Include("CreatedByUser").Include("shelves").Include("taskstatuses").Include("LastUpdatedUser").Include("taskcategories")
						 where i.shelves.ShelveId == editionId
						 select new TaskDTO
							 {
								 assetid = i.assets != null ? i.assets.AssetId : (Guid?)null,
								 assignToUserId = i.AssignToUser != null ? i.AssignToUser.UserId : (Guid?)null,
								 category = i.taskcategories != null ?i.taskcategories.name : null,
								 categoryId = i.taskcategories != null ? i.taskcategories.id : (int?)null,
								 description = i.description,
								 editionId = i.shelves.ShelveId,
								 page = i.page,
								 spread = i.spread,
								 subject = i.subject,
								 taskid = i.taskid,
								 taskStatusId = i.taskstatuses.taskstatusid,
								 versionNumber = i.versionnumber.HasValue ? i.versionnumber.Value : 0,
								 TaskUpdateInfo = new TaskUpdateInfo { createdByUserid = i.CreatedByUser.UserId,
													CreationDate = i.creationdatetime,
												   lastModifiedDate = i.lastupdatedatetime,
												   lastModifiedUserId = i.LastUpdatedUser != null ?i.LastUpdatedUser.UserId : (Guid?)null,
												   assigntoUserName = i.AssignToUser != null ? i.AssignToUser.FirstName + " " + i.AssignToUser.LastName : "",
													createdByUserName = i.CreatedByUser !=null?i.CreatedByUser.FirstName + " " + i.CreatedByUser.LastName :"",
													lastModifiedUserName = i.LastUpdatedUser != null ? i.LastUpdatedUser.FirstName + " " + i.LastUpdatedUser.LastName : ""
								 }
							 }).ToList();
			}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing GetTasks  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
		
		
		
		
		return (tasks.ToArray());
	}

	

	/// <summary>
	/// [29.04.10 Prakash Bhatt]
	/// Returns an array of edditins of spicified title.
	/// </summary>
	/// <param name="titleid"></param>
	/// <returns></returns>
	public EditionTitlesDTO[] GetEditionsByTitle(Guid titleid)
	{
		List<EditionTitlesDTO> editionList;
		EditionTitlesDTO newEdition;
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>();
			iZINE.Businesslayer.User user = context.Users.Include("Titles").Include("Titles.Shelves").FirstOrDefault(u => u.Membership.Username.CompareTo(OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name) == 0 && u.Active == true) as User;
			if (user == null)
				throw new Exception("User account not found");

			var editions = from s in user.Titles
							where s.Active == true && s.Shelves.Count > 0 //&& s.TitleId == titleid
							orderby s.Name
							select new
							{
								titleid = s.TitleId,
								edition = from n in s.Shelves
										 where n.Active == true
										 select new ShelveDTO
										 {
											 name = n.Name,
											 shelveid = n.ShelveId
										 }
							};

			if (titleid != Guid.Empty)
				editions.Where(s => s.titleid == titleid);

			// If query results null then return null.
			if (editions == null)
				return null;

			editionList = new List<EditionTitlesDTO>();
			foreach (var edition in editions)
			{
				newEdition = new EditionTitlesDTO();
				newEdition.titleid = edition.titleid;
				newEdition.editions = edition.edition.ToArray<ShelveDTO>();
				editionList.Add(newEdition);
				newEdition = null;
			}

			return editionList.ToArray();
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing GetEditionByTitle  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	

	/// <summary>
	/// [29.04.10 Prakash Bhatt]
	/// Returns Array of status for specified titles.
	/// </summary>
	/// <param name="titleid">Specify TitleID as a parameter.</param>
	/// <returns></returns>
	public TitleStatusDTO[] GetStatusByTitle(Guid titleid)
	{
		List<TitleStatusDTO> statusList;
		TitleStatusDTO newStatus;
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>();
			iZINE.Businesslayer.User user = context.Users.Include("Titles").Include("Titles.title_status").Include("Titles.title_status.status").Include("Titles.title_status.status.StatusStates").FirstOrDefault(u => u.Membership.Username.CompareTo(OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name) == 0 && u.Active == true) as User;
			if (user == null)
				throw new Exception("User account not found");
			
			var status = from n in user.Titles
						 where n.Active == true 
						 select new
						 {
							titleid = n.TitleId,
							statuses = from m in n.title_status
										where m.active == true
										select new StatusDTO
										{
										  statusId = m.status.StatusId,
										  statusName = m.status.Name,
										  layout = m.layout,
										  text = m.text,
										  stateId = m.status.StatusStates.StateId
										}
						 };

			if (titleid != Guid.Empty)
				status = status.Where(n => n.titleid == titleid);


			if (status == null)
				return null;

			statusList = new List<TitleStatusDTO>();

			foreach (var item in status)
			{
				newStatus = new TitleStatusDTO();
				newStatus.statuses = item.statuses.ToArray<StatusDTO>();
				newStatus.titleid = item.titleid;
				statusList.Add(newStatus);
			}

			return statusList.ToArray<TitleStatusDTO>();
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing GetStatusByTitle  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

    public void UpdateAssetStatus(Guid assetid, Guid oldstatusid, Guid newstatusid)
    {
        int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
        try
        {
            using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
            {

                Asset asset = (from a in context.Assets.Include("Version")
                               where a.AssetId == assetid
                               select a).FirstOrDefault();

                if (asset == null)
                    throw new Exception("asset not found");

                if (asset.Version.Status.StatusId != oldstatusid)
                    throw new Exception("Status has been changed");

                iZINE.Businesslayer.Status status = context.Statuses.FirstOrDefault<iZINE.Businesslayer.Status>(v => v.StatusId == assetdto.Head.statusid) as Status;
                if (status != null && asset.Version.status != status)
                    asset.Version.status = status;

                /*
                if (asset.
                iZINE.Businesslayer.Status status = context.Statuses.FirstOrDefault<iZINE.Businesslayer.Status>(v => v.StatusId == assetdto.Head.statusid) as Status;
                //if (status == null)
                //	throw new Exception("status not found");

                iZINE.Businesslayer.Shelve shelve = context.Shelves.FirstOrDefault<iZINE.Businesslayer.Shelve>(v => v.ShelveId == assetdto.shelveid) as Shelve;
                //if (shelve == null)
                //	throw new Exception("shelve not found");

                iZINE.Businesslayer.Title title = context.Titles.FirstOrDefault<iZINE.Businesslayer.Title>(v => v.TitleId == assetdto.titleid) as Title;
                //if (title == null)
                //	throw new Exception("title not found");

                asset.Name = assetdto.name;
                if (shelve != null)
                    asset.Shelve = shelve;
                if (title != null)
                    asset.Title = title;
                if (status != null && asset.Version.status != status)
                    asset.Version.status = status;
                */
                context.SaveChanges();
            }
        }
        catch (Exception exc)
        {
            log.Error(exc);
            throw;
        }
        finally
        {
            int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
            log.InfoFormat("finished processing UpdateAsset  in {0} seconds", (iTotalEndTime - iTotalStartTime));
        }
    }

	public void UpdateAsset(Guid assetid, AssetDTO assetdto)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{

				Asset asset = (from a in context.Assets.Include("Version")
								where a.AssetId == assetid
								select a).FirstOrDefault();

				if (asset == null)
					throw new Exception("asset not found");

				iZINE.Businesslayer.Status status = context.Statuses.FirstOrDefault<iZINE.Businesslayer.Status>(v => v.StatusId == assetdto.Head.statusid) as Status;
				//if (status == null)
				//	throw new Exception("status not found");

				iZINE.Businesslayer.Shelve shelve = context.Shelves.FirstOrDefault<iZINE.Businesslayer.Shelve>(v => v.ShelveId == assetdto.shelveid) as Shelve;
				//if (shelve == null)
				//	throw new Exception("shelve not found");

				iZINE.Businesslayer.Title title = context.Titles.FirstOrDefault<iZINE.Businesslayer.Title>(v => v.TitleId == assetdto.titleid) as Title;
				//if (title == null)
				//	throw new Exception("title not found");

				asset.Name = assetdto.name;
				if( shelve != null)
					asset.Shelve = shelve;
				if (title != null)
					asset.Title = title;
				if (status != null && asset.Version.status != status)
					asset.Version.status = status;
				context.SaveChanges();
			}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing UpdateAsset  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	public AssetDTO[] GetAssetList(Guid shelveid, Guid[] types)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{
				
				// compiling query for performance
				Func<iZINEEntities, Guid, Guid, IQueryable<Asset>> GetAssetsByShelveIdAndTypeId =
					CompiledQuery.Compile((iZINEEntities entities, Guid shelveid2, Guid typeid2) =>
					(
						from a in entities.Assets.Include("Document").Include("Shelve").Include("Title").Include("Type").Include("Version").Include("Lock").Include("Version.Status").Include("Version.User").Include("Version.User.Membership")
						where a.Active == true && a.Shelve.ShelveId == shelveid2 && a.Type.ConstantId == typeid2
						select a
					)
				);
				// compiling query for performance
				Func<iZINEEntities, Guid, IQueryable<Asset>> GetAssetsByShelveId =
					CompiledQuery.Compile((iZINEEntities entities, Guid shelveid2) =>
					(
						from a in entities.Assets.Include("Document").Include("Shelve").Include("Title").Include("Version").Include("Lock").Include("Version.Status").Include("Version.User").Include("Version.User.Membership")
						where a.Active == true && a.Shelve.ShelveId == shelveid2 
						select a
					)
				);

				List<AssetDTO> assetList = new List<AssetDTO>();

				if (types.Length > 0)
				{
					// it is a damn ugly expression, but it works
					foreach (Guid type in types)
					{
						// Include("Lock") removed, 
						IEnumerable<AssetDTO> list = from b in
														 (
															GetAssetsByShelveIdAndTypeId(context, shelveid, type).AsEnumerable()
														 )
													 select new AssetDTO
													 {
														 assetid = b.AssetId,
														 name = b.Name,
														 documentid = (b.Document != null ? b.Document.AssetId : (Guid?)null),
														 documentname = (b.Document != null ? b.Document.Name : null),
														 titleid = (b.Title != null ? b.Title.TitleId : (Guid?)null),
														 shelveid = (b.Shelve != null ? b.Shelve.ShelveId : (Guid?)null),
														 type = (b.Type != null ? b.Type.Name : null),
														 typeid = (b.Type != null ? b.Type.ConstantId : (Guid?)null),
														 lockid = (b.Lock != null ? b.Lock.LockId : (Guid?)null),
														 Head = new VersionInfo
														 {
															 versionid = (b.Version != null ? b.Version.VersionId : (Guid?)null),
															 number = (b.Version != null ? b.Version.Number : null),
															 date = (b.Version != null ? b.Version.Date.ToUniversalTime() : (DateTime?)null),
															 statusid = (b.Version != null && b.Version.status != null ? b.Version.status.StatusId : (Guid?)null),

															 user = (b.Version != null && b.Version.User != null && b.Version.User.Membership != null
																		 ? new UserDTO
																		 {
																			 userid = b.Version.User.UserId,
																			 name = b.Version.User.Fullname,
																			 username = b.Version.User.Membership.Username,
																		 }
																			 : null
																 )
														 }

													 };

						assetList.AddRange(list.ToArray<AssetDTO>());
					}
				}
				else
				{
					IEnumerable<AssetDTO> list = from b in
													 (
														GetAssetsByShelveId(context, shelveid).AsEnumerable()
													 )
												 select new AssetDTO
												 {
													 assetid = b.AssetId,
													 name = b.Name,
													 documentid = (b.Document != null ? b.Document.AssetId : (Guid?)null),
													 documentname = (b.Document != null ? b.Document.Name : null),
													 titleid = (b.Title != null ? b.Title.TitleId : (Guid?)null),
													 shelveid = (b.Shelve != null ? b.Shelve.ShelveId : (Guid?)null),
													 type = (b.Type != null ? b.Type.Name : null),
													 typeid = (b.Type != null ? b.Type.ConstantId : (Guid?)null),
													 lockid = (b.Lock != null ? b.Lock.LockId : (Guid?)null),

													 Head = new VersionInfo
													 {
														 versionid = (b.Version != null ? b.Version.VersionId : (Guid?)null),
														 number = (b.Version != null ? b.Version.Number : null),
														 date = (b.Version != null ? b.Version.Date.ToUniversalTime() : (DateTime?)null),
														 statusid = (b.Version != null && b.Version.status != null ? b.Version.status.StatusId : (Guid?)null),

														 user = (b.Version != null && b.Version.User != null && b.Version.User.Membership != null
																	 ? new UserDTO
																	 {
																		 userid = b.Version.User.UserId,
																		 name = b.Version.User.Fullname,
																		 username = b.Version.User.Membership.Username,
																	 }
																		 : null
															 )
													 }

												 };

					assetList.AddRange(list.ToArray<AssetDTO>());
				}

				return (from a
							in assetList
						orderby a.Head.date descending
						select a)
						.ToArray<AssetDTO>();
			}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing GetAssetList  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	public LockDTO[] GetLock(Guid[] assetids)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			LockDTO[] LockDTOs = new LockDTO[assetids.Length];
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{

				Func<iZINEEntities, Guid, Asset> GetAssetById =
								 CompiledQuery.Compile((iZINEEntities entities, Guid assetid2) =>
								 (
									 (
										 from a in entities.Assets.Include("Lock")
										 where a.AssetId == assetid2
										 select a
									 ).FirstOrDefault<Asset>()
								 )
							 );
				for (int i = 0; i < assetids.Length; i++)
				{
					Asset asset = GetAssetById(context, assetids[i]);

					if (asset == null)
						continue;

					LockDTO lockDTO = null;
					
					if (asset.Lock == null)
					{
						lockDTO = new LockDTO { assetid = asset.AssetId };
					}
					else
					{

						asset.Lock.OwnerReference.Load();
						asset.Lock.Owner.MembershipReference.Load();

						lockDTO = (new LockDTO
						{
							lockid = asset.Lock.LockId,
							applicationname = asset.Lock.ApplicationName,
							documentname = asset.Lock.DocumentName,
							username = asset.Lock.Owner.Membership.Username,
							assetid = asset.AssetId,
							userid = asset.Lock.Owner.Membership.UserId
						}
									 );
					}
					LockDTOs[i] = lockDTO;
				}
				return (LockDTOs);
			}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing GetLock  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="assetid"></param>
	/// <param name="statusid"></param>
	/// <param name="comment"></param>
	/// <param name="versionId"></param>
	/// <param name="headVersion">if headversion is zero then check is force else it will be compared to current head</param>
	/// <returns></returns>
	public AssetDTO CheckInAsset(Guid assetid, Guid statusid, String comment, Guid versionId,int headVersion)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);

		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{

				iZINE.Businesslayer.Asset asset = context.Assets.Include("Lock").FirstOrDefault<iZINE.Businesslayer.Asset>(a => a.AssetId == assetid);
				if (asset == null)
					throw new Exception("asset not found");

				asset.VersionReference.Load();

				//if headversion in not zero then it's asset might have head
				if (headVersion != 0 && asset.Version.Number != headVersion)
				{
					throw new Exception("checking-in performed with updating local copy to latest version");
				}
				iZINE.Businesslayer.User user = context.Users.FirstOrDefault(u => u.Membership.Username.CompareTo(OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name) == 0 && u.Active == true) as User;

				if (asset.Version != null)
				{
					asset.Lock.OwnerReference.Load();
					asset.Lock.Owner.MembershipReference.Load();

					
					if (!asset.Lock.Owner.Equals(user))
						throw new Exception("Asset is locked by defferent user");
				}
				iZINE.Businesslayer.Status status = context.Statuses.FirstOrDefault<iZINE.Businesslayer.Status>(v => v.StatusId == statusid) as Status;

				iZINE.Businesslayer.Version version = context.Versions.FirstOrDefault<iZINE.Businesslayer.Version>(v => v.VersionId == versionId) as iZINE.Businesslayer.Version;

				if (version == null)
					throw new Exception("version not found");

				if( asset.Version == null )
					version.Number = 1;
				else
					version.Number = asset.Version.Number + 1;

				asset.Version = version;
				asset.Versions.Add(version);
				asset.VersionReference.Load();

				asset.Version.status = status;

				Comment c = new Comment();
				c.CommentId = Guid.NewGuid();
				c.Text = comment;
				c.User = user;
				c.Active = true;
				c.Asset = asset;
				// c.Version = asset.Version;
				c.Date = DateTime.Now;
				context.AddToComments(c);

				context.SaveChanges();

				user.MembershipReference.Load();
				asset.TitleReference.Load();
				asset.Title.Users.Load();
				asset.Title.Notifications.Load();
				asset.ShelveReference.Load();
				asset.TypeReference.Load();

				if (asset.Type.ConstantId == new Guid("b4eee6f9-6061-481e-8a6e-f89dfbebad4e"))
				{
					// send notification (only for assignments)

					log.Debug(asset.Title.Users.Count.ToString());

					foreach (UserNotification notification in asset.Title.Notifications)
					{
						notification.UserReference.Load();
						notification.User.MembershipReference.Load();

						if (notification.StatusId.CompareTo(statusid) == 0)
						{
							try
							{
								System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
								message.Subject = asset.Title.Name + " " + asset.Shelve.Name + ": Status " + asset.Name + " changed to " + status.Name;
								message.From = new System.Net.Mail.MailAddress("notifications@izine-publish.net", "iZine Notifications");
								message.Body = "The status of " + asset.Name + " has been updated to " + status.Name + " by user " + user.Fullname + "." + "\n" + comment;

								message.To.Add(new MailAddress(notification.User.Membership.Email));

								new System.Net.Mail.SmtpClient().Send(message);

							}
							catch (Exception exc)
							{
								log.Error(exc);
							}
						}
					}
				}
				return new AssetDTO
				{
					assetid = asset.AssetId,
					documentid = asset.Document == null ?
						(Guid?)null : asset.Document.AssetId,
					documentname = asset.Document == null ?
						null : asset.Document.Name,
					Head = new VersionInfo
					{
						versionid = (asset.Version != null ? asset.Version.VersionId : (Guid?)null),
						number = (asset.Version != null ? asset.Version.Number : null),
						date = (asset.Version != null ? (DateTime)asset.Version.Date.ToUniversalTime() : (DateTime?)null),
						statusid = (asset.Version != null && asset.Version.status != null ? asset.Version.status.StatusId : (Guid?)null),

						user = (asset.Version != null && asset.Version.User != null && asset.Version.User.Membership != null
									? new UserDTO
									{
										userid = asset.Version.User.UserId,
										name = asset.Version.User.Fullname,
										username = asset.Version.User.Membership.Username,
									}
										: null
							)
					},
					lockid = (Guid?)null,
					name = asset.Name,
					shelveid = asset.Shelve.ShelveId,
					titleid = asset.Title.TitleId,
					type = asset.Type.Name,
					typeid = asset.Type.ConstantId
				};
			}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing check-in  in {0} seconds..", (iTotalEndTime - iTotalStartTime));
		}
	}
/*
	public VersionDTO DownloadVersion(Guid versionid)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{
				VersionDTO versionDto = new VersionDTO();

				iZINE.Businesslayer.Version version = context.Versions.FirstOrDefault<iZINE.Businesslayer.Version>(v => v.VersionId == versionid);
				if (version == null)
					throw new Exception("version not found");

				version.AssetReference.Load();
				version.Asset.TitleReference.Load();
				version.Asset.ShelveReference.Load();
				
				string assetsDirAmazon = @"data\assets";

				String assetsDir = System.Configuration.ConfigurationManager.AppSettings["assetsDir"];

				assetsDir = Path.Combine(assetsDir, String.Format("{0}/{1}/{2}", version.Asset.Title.TitleId, version.Asset.Shelve.Date.Year.ToString(), version.Asset.Shelve.ShelveId.ToString()));

				assetsDirAmazon =   assetsDirAmazon + String.Format(@"\{0}\{1}\{2}\", version.Asset.Title.TitleId, version.Asset.Shelve.Date.Year.ToString(), version.Asset.Shelve.ShelveId.ToString());

				version.Asset.TypeReference.Load();


				string pathAmazon   =   "";
				string pathLocal = "";


				MemoryStream ms = null;
				string fileName	= "";

				if (version.Asset.Type.ConstantId == new Guid("a3871d22-7d8b-4d9f-901e-4c284b1a801e"))
				{

			fileName	=	 String.Format("{0}.indd", version.VersionId.ToString());
					pathAmazon   =   Path.Combine(assetsDirAmazon,fileName	);
					pathLocal   =      Path.Combine(assetsDir, fileName);

                    if (File.Exists(pathLocal))
            				{
                                ms = new MemoryStream(System.IO.File.ReadAllBytes(pathLocal));
            				}
            				else
            				{
                
                				Stream sm = GetFileContents(pathAmazon.Replace("\\", "/"));
                				ms = new MemoryStream();
                				sm.CopyTo(ms, 0x2000);
            				}
					// save file with versionid!
					//ms = new MemoryStream(System.IO.File.ReadAllBytes(Path.Combine(assetsDir, String.Format("{0}.indd", version.VersionId.ToString()))));
				}
				else if (version.Asset.Type.ConstantId == new Guid("BDEC0BF8-9591-4533-AFEA-467D0B340E92"))
				{
					// save file with versionid!
					ms = new MemoryStream(System.IO.File.ReadAllBytes(Path.Combine(assetsDir, String.Format("{0}.incx", version.VersionId.ToString()))));
				}
				else if (version.Asset.Type.ConstantId == new Guid("756779a5-bb56-4430-9e95-022226970095"))
				{
					// save file with versionid!
					ms = new MemoryStream(System.IO.File.ReadAllBytes(Path.Combine(assetsDir, String.Format("{0}.incx", version.VersionId.ToString()))));
				}
				else if (version.Asset.Type.ConstantId == new Guid("0DB0B0BC-25A8-4B5E-81B1-819336832D68"))
				{
					// save file with versionid!
					ms = new MemoryStream(System.IO.File.ReadAllBytes(Path.Combine(assetsDir, String.Format("{0}.inct", version.VersionId.ToString()))));
				}
				else if (version.Asset.Type.ConstantId == new Guid("2326e21b-f4fb-40eb-acf6-46454d4d9f9f"))
				{
					// save file with versionid!
					ms = new MemoryStream(System.IO.File.ReadAllBytes(Path.Combine(assetsDir, String.Format("{0}.indt", version.VersionId.ToString()))));
				}
				else if (version.Asset.Type.ConstantId == new Guid("b4eee6f9-6061-481e-8a6e-f89dfbebad4e"))
				{
					// save file with versionid!
					ms = new MemoryStream(System.IO.File.ReadAllBytes(Path.Combine(assetsDir, String.Format("{0}.inca", version.VersionId.ToString()))));
				}

				// not the most beautifull code, should be replace by return stream
				// instead of byte array

				versionDto.isCompressed = true;//TODO: when web client understand turn it on
				if (versionDto.isCompressed)
				{
					MemoryStream os = new MemoryStream();
					Stream outs = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(os);
					// Stream outs = os;

					byte[] buffer = new byte[8192];
					int bytesRead = ms.Read(buffer, 0, 8192);

					for (; bytesRead != 0; )
					{
						outs.Write(buffer, 0, bytesRead);
						bytesRead = ms.Read(buffer, 0, 8192);
					}
					outs.Flush();
					outs.Close();	//Close it before taking output, even flush is not giving all data.

					versionDto.data = os.ToArray();
					os.Close();
				}
				else
				{
					versionDto.data = ms.ToArray();
				}
				
				versionDto.versionId = versionid;

				return versionDto;
								
			}

		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing DownLoadVersion  in {0} seconds...", (iTotalEndTime - iTotalStartTime));
		}
	}
*/

public VersionDTO DownloadVersion(Guid versionid)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{

				VersionDTO versionDto = new VersionDTO();

				iZINE.Businesslayer.Version version = context.Versions.FirstOrDefault<iZINE.Businesslayer.Version>(v => v.VersionId == versionid);
				if (version == null)
					throw new Exception("version not found");

				version.AssetReference.Load();
				version.Asset.TitleReference.Load();
				version.Asset.ShelveReference.Load();
				
				string assetsDirAmazon = @"data\assets";

				String assetsDir = System.Configuration.ConfigurationManager.AppSettings["assetsDir"];

				assetsDir = Path.Combine(assetsDir, String.Format("{0}/{1}/{2}", version.Asset.Title.TitleId, version.Asset.Shelve.Date.Year.ToString(), version.Asset.Shelve.ShelveId.ToString()));

				assetsDirAmazon =   assetsDirAmazon + String.Format(@"\{0}\{1}\{2}\", version.Asset.Title.TitleId, version.Asset.Shelve.Date.Year.ToString(), version.Asset.Shelve.ShelveId.ToString());

				version.Asset.TypeReference.Load();


				string pathAmazon   =   "";
				string pathLocal = "";


				MemoryStream ms = null;
                string fileName	= "";

				if (version.Asset.Type.ConstantId == new Guid("a3871d22-7d8b-4d9f-901e-4c284b1a801e"))
				{
					fileName	=	 String.Format("{0}.indd", version.VersionId.ToString());
					pathAmazon   =   Path.Combine(assetsDirAmazon,fileName	);
					pathLocal   =      Path.Combine(assetsDir, fileName);

                    if (File.Exists(pathLocal))
            				{
                                ms = new MemoryStream(System.IO.File.ReadAllBytes(pathLocal));
            				}
            				else
            				{
                
                				Stream sm = GetFileContents(pathAmazon.Replace("\\", "/"));
                				ms = new MemoryStream();
                				sm.CopyTo(ms, 0x2000);
            				}
					// save file with versionid!
					//ms = new MemoryStream(System.IO.File.ReadAllBytes(Path.Combine(assetsDir, String.Format("{0}.indd", version.VersionId.ToString()))));
				}
				else if (version.Asset.Type.ConstantId == new Guid("BDEC0BF8-9591-4533-AFEA-467D0B340E92"))
				{
                    fileName = String.Format("{0}.incx", version.VersionId.ToString());
                    pathAmazon = Path.Combine(assetsDirAmazon, fileName);
                    pathLocal = Path.Combine(assetsDir, fileName);

                    if (File.Exists(pathLocal))
                    {
                        ms = new MemoryStream(System.IO.File.ReadAllBytes(pathLocal));
                    }
                    else
                    {

                        Stream sm = GetFileContents(pathAmazon.Replace("\\", "/"));
                        ms = new MemoryStream();
                        sm.CopyTo(ms, 0x2000);
                    }
					// save file with versionid!
					//ms = new MemoryStream(System.IO.File.ReadAllBytes(Path.Combine(assetsDir, String.Format("{0}.incx", version.VersionId.ToString()))));
				}
				else if (version.Asset.Type.ConstantId == new Guid("756779a5-bb56-4430-9e95-022226970095"))
				{
                    fileName = String.Format("{0}.incx", version.VersionId.ToString());
                    pathAmazon = Path.Combine(assetsDirAmazon, fileName);
                    pathLocal = Path.Combine(assetsDir, fileName);

                    if (File.Exists(pathLocal))
                    {
                        ms = new MemoryStream(System.IO.File.ReadAllBytes(pathLocal));
                    }
                    else
                    {

                        Stream sm = GetFileContents(pathAmazon.Replace("\\", "/"));
                        ms = new MemoryStream();
                        sm.CopyTo(ms, 0x2000);
                    }

					// save file with versionid!
					//ms = new MemoryStream(System.IO.File.ReadAllBytes(Path.Combine(assetsDir, String.Format("{0}.incx", version.VersionId.ToString()))));
				}
				else if (version.Asset.Type.ConstantId == new Guid("0DB0B0BC-25A8-4B5E-81B1-819336832D68"))
				{
                    fileName = String.Format("{0}.inct", version.VersionId.ToString());
                    pathAmazon = Path.Combine(assetsDirAmazon, fileName);
                    pathLocal = Path.Combine(assetsDir, fileName);

                    if (File.Exists(pathLocal))
                    {
                        ms = new MemoryStream(System.IO.File.ReadAllBytes(pathLocal));
                    }
                    else
                    {

                        Stream sm = GetFileContents(pathAmazon.Replace("\\", "/"));
                        ms = new MemoryStream();
                        sm.CopyTo(ms, 0x2000);
                    }

					// save file with versionid!
					// ms = new MemoryStream(System.IO.File.ReadAllBytes(Path.Combine(assetsDir, String.Format("{0}.inct", version.VersionId.ToString()))));
				}
				else if (version.Asset.Type.ConstantId == new Guid("2326e21b-f4fb-40eb-acf6-46454d4d9f9f"))
				{
                    fileName = String.Format("{0}.indt", version.VersionId.ToString());
                    pathAmazon = Path.Combine(assetsDirAmazon, fileName);
                    pathLocal = Path.Combine(assetsDir, fileName);

                    if (File.Exists(pathLocal))
                    {
                        ms = new MemoryStream(System.IO.File.ReadAllBytes(pathLocal));
                    }
                    else
                    {

                        Stream sm = GetFileContents(pathAmazon.Replace("\\", "/"));
                        ms = new MemoryStream();
                        sm.CopyTo(ms, 0x2000);
                    }
					// save file with versionid!
					//ms = new MemoryStream(System.IO.File.ReadAllBytes(Path.Combine(assetsDir, String.Format("{0}.indt", version.VersionId.ToString()))));
				}
				else if (version.Asset.Type.ConstantId == new Guid("b4eee6f9-6061-481e-8a6e-f89dfbebad4e"))
				{
                    fileName = String.Format("{0}.inca", version.VersionId.ToString());
                    pathAmazon = Path.Combine(assetsDirAmazon, fileName);
                    pathLocal = Path.Combine(assetsDir, fileName);

                    if (File.Exists(pathLocal))
                    {
                        ms = new MemoryStream(System.IO.File.ReadAllBytes(pathLocal));
                    }
                    else
                    {

                        Stream sm = GetFileContents(pathAmazon.Replace("\\", "/"));
                        ms = new MemoryStream();
                        sm.CopyTo(ms, 0x2000);
                    }

					// save file with versionid!
					//ms = new MemoryStream(System.IO.File.ReadAllBytes(Path.Combine(assetsDir, String.Format("{0}.inca", version.VersionId.ToString()))));
				}

				// not the most beautifull code, should be replace by return stream
				// instead of byte array

				versionDto.isCompressed = true;//TODO: when web client understand turn it on
				if (versionDto.isCompressed)
				{
					MemoryStream os = new MemoryStream();
					Stream outs = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(os);
					// Stream outs = os;

					byte[] buffer = new byte[8192];
					int bytesRead = ms.Read(buffer, 0, 8192);

					for (; bytesRead != 0; )
					{
						outs.Write(buffer, 0, bytesRead);
						bytesRead = ms.Read(buffer, 0, 8192);
					}
					outs.Flush();
					outs.Close();	//Close it before taking output, even flush is not giving all data.

					versionDto.data = os.ToArray();
					os.Close();
				}
				else
				{
					versionDto.data = ms.ToArray();
				}
				
				versionDto.versionId = versionid;

				return versionDto;
								
			}

		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing DownLoadVersion  in {0} seconds..", (iTotalEndTime - iTotalStartTime));
		}
	}


  public static  Stream GetFileContents(String key)
        {
            AmazonS3Config config = new AmazonS3Config();
            config.ServiceURL = ConfigurationManager.AppSettings["AWSServiceUrl"];//s3-eu-west-1.amazonaws.com";

            using (AmazonS3 s3client = Amazon.AWSClientFactory.CreateAmazonS3Client(ConfigurationManager.AppSettings["AWSAccessKey"], ConfigurationManager.AppSettings["AWSSecretKey"], config))
            {
                MemoryStream file = new MemoryStream();
                try
                {
                    GetObjectRequest req = new GetObjectRequest()
                    .WithBucketName(ConfigurationManager.AppSettings["AWSBucketName"])
                   .WithKey(key);

                    GetObjectResponse res = s3client.GetObject(req);
                    try
                    {
                        return res.ResponseStream;
                        /*
                        long transferred = 0L;
                        BufferedStream stream2 = new BufferedStream(res.ResponseStream);
                        
                       
                        byte[] buffer = new byte[0x2000];
                        int count = 0;
                        while ((count = stream2.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            file.Write(buffer, 0, count);
                        }
                         */
                    }
                    finally
                    {
                    }
                    //  return file;
                }
                catch (AmazonS3Exception ex)
                {
			log.Error(ex);
                    return null;
                    //Show exception
                }
            }
        }

	public UserDTO[] GetUsersForTitle(Guid titleId)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		UserDTO[] userDTOs = null;
		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{
				Title title = context.Titles.Include("Users").Where(u => u.TitleId == titleId).FirstOrDefault();
				if (title == null)
				{
					throw new Exception("title id is not valid");
				}

				userDTOs = (from u in title.Users select new UserDTO { name = u.Fullname, userid = u.UserId }).ToArray();
				
			}
		}
		catch (Exception exc)
		{

			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing GetUserForTitle  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
		return userDTOs;
	}

	public IdNameDTO[] GetTaskStatusList()
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		
		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{
				return (from i in context.taskstatuses select new IdNameDTO { id = i.taskstatusid, name = i.name }).ToArray();
			}

		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing GetTaskStatusList  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	public IdNameDTO[] GetTaskCategoryList()
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);

		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{
				return (from i in context.taskcategories select new IdNameDTO { id = i.id, name = i.name }).ToArray();
			}

		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing GetTaskCategoryList  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}

	public TaskDTO UpdateTask(TaskDTO task,string comment)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		TaskDTO taskToReturn = new TaskDTO();
		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{
				DateTime currentDateTime = DateTime.Now;
				iZINE.Businesslayer.User user = context.Users.FirstOrDefault(u => u.Membership.Username.CompareTo(OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name) == 0 && u.Active == true) as User;
				tasks taskOriginal = context.tasks.FirstOrDefault(t=>t.taskid == task.taskid.Value);
				int versionNumber = taskOriginal.versionnumber.HasValue ? taskOriginal.versionnumber.Value + 1 : 1;

				if (task.versionNumber + 1 != versionNumber)
				{
					throw new Exception("version number is different than the databse value");
				}
				//find the updated column and insert rows 
				Asset newAsset = null;
				User newAssignToUser = null;
				//Shelve newShelve = null;
				taskcategories newCategories = null;
				//taskstatuses newStatus = null;
				taskOriginal.assetsReference.Load();
				taskOriginal.AssignToUserReference.Load();
				taskOriginal.CreatedByUserReference.Load();
				taskOriginal.LastUpdatedUserReference.Load();
				taskOriginal.shelvesReference.Load();
				taskOriginal.taskcategoriesReference.Load();
				taskOriginal.taskstatusesReference.Load();
				if (task.assetid.HasValue)
					newAsset = context.Assets.FirstOrDefault(a => a.AssetId == task.assetid.Value);
				if (task.assignToUserId.HasValue)
					newAssignToUser = context.Users.FirstOrDefault(u => u.UserId == task.assignToUserId);
				if (task.categoryId.HasValue)
					newCategories = context.taskcategories.FirstOrDefault(c => c.id == task.categoryId);
				
				if (!IsEqual(task.assetid,(taskOriginal.assets != null ? taskOriginal.assets.AssetId:(Guid?)null)))
				{
					taskhistory history = new taskhistory();
					history.modifiedcolumn = (int)enTaskColumns.eDOCUMENTID;
					history.newvalue = task.assetid.HasValue ? task.assetid.ToString(): null;
					history.oldvalue = taskOriginal.assets != null ? taskOriginal.assets.AssetId.ToString(): null;
					history.tasks = taskOriginal;
					history.timestamp = currentDateTime;
					history.users = user;
					history.versionnumber = versionNumber;
					history.taskhistoryid = Guid.NewGuid();
					context.AddTotaskhistory(history);

					taskOriginal.assets = newAsset;
				}

				if (!IsEqual(task.assignToUserId,taskOriginal.AssignToUser != null ? taskOriginal.AssignToUser.UserId :(Guid?) null))
				{
					taskhistory history = new taskhistory();
					history.modifiedcolumn = (int)enTaskColumns.eASSIGNTO;
					history.newvalue = task.assignToUserId.HasValue ? task.assignToUserId.Value.ToString() : null;
					history.oldvalue = taskOriginal.AssignToUser != null ?taskOriginal.AssignToUser.UserId.ToString() : null;
					history.tasks = taskOriginal;
					history.timestamp = currentDateTime;
					history.users = user;
					history.versionnumber = versionNumber;
					history.taskhistoryid = Guid.NewGuid();
					context.AddTotaskhistory(history);

					taskOriginal.AssignToUser = newAssignToUser;
				}

				if (!IsEqual(task.description,taskOriginal.description))
				{
					taskhistory history = new taskhistory();
					history.modifiedcolumn = (int)enTaskColumns.eDESCRIPTION;
					history.newvalue = task.description;
					history.oldvalue = taskOriginal.description;
					history.tasks = taskOriginal;
					history.timestamp = currentDateTime;
					history.users = user;
					history.versionnumber = versionNumber;
					history.taskhistoryid = Guid.NewGuid();
					context.AddTotaskhistory(history);

					taskOriginal.description = task.description;
				}

				if ((task.editionId.CompareTo(taskOriginal.shelves.ShelveId) != 0))
				{
					taskhistory history = new taskhistory();
					history.modifiedcolumn = (int)enTaskColumns.eEDITIONID;
					history.newvalue = task.editionId.ToString();
					history.oldvalue = taskOriginal.shelves.ShelveId.ToString();
					history.tasks = taskOriginal;
					history.timestamp = currentDateTime;
					history.users = user;
					history.versionnumber = versionNumber;
					history.taskhistoryid = Guid.NewGuid();
					context.AddTotaskhistory(history);

					taskOriginal.shelves = context.Shelves.FirstOrDefault(a => a.ShelveId == task.editionId);
				}
				if (!IsEqual(task.page,taskOriginal.page) )
				{
					taskhistory history = new taskhistory();
					history.modifiedcolumn = (int)enTaskColumns.ePAGE;
					history.newvalue = task.page;
					history.oldvalue = taskOriginal.page;
					history.tasks = taskOriginal;
					history.timestamp = currentDateTime;
					history.users = user;
					history.versionnumber = versionNumber;
					history.taskhistoryid = Guid.NewGuid();
					context.AddTotaskhistory(history);

					taskOriginal.page = task.page;
				}
				if (!IsEqual(task.spread , taskOriginal.spread))
				{
					taskhistory history = new taskhistory();
					history.modifiedcolumn = (int)enTaskColumns.eSPREAD;
					history.newvalue = task.spread;
					history.oldvalue = taskOriginal.spread;
					history.tasks = taskOriginal;
					history.timestamp = currentDateTime;
					history.users = user;
					history.versionnumber = versionNumber;
					history.taskhistoryid = Guid.NewGuid();
					context.AddTotaskhistory(history);

					taskOriginal.spread = task.spread;
				}

				if (!IsEqual(task.subject , taskOriginal.subject))
				{
					taskhistory history = new taskhistory();
					history.modifiedcolumn = (int)enTaskColumns.eSUBJECT;
					history.newvalue = task.subject;
					history.oldvalue = taskOriginal.subject;
					history.tasks = taskOriginal;
					history.timestamp = currentDateTime;
					history.users = user;
					history.versionnumber = versionNumber;
					history.taskhistoryid = Guid.NewGuid();
					context.AddTotaskhistory(history);

					taskOriginal.subject = task.subject;
				}

				if ((task.taskStatusId.CompareTo(taskOriginal.taskstatuses.taskstatusid) != 0))
				{
					taskhistory history = new taskhistory();
					history.modifiedcolumn = (int)enTaskColumns.eSTAUSID;
					history.newvalue = task.taskStatusId.ToString();
					history.oldvalue = taskOriginal.taskstatuses.taskstatusid.ToString();
					history.tasks = taskOriginal;
					history.timestamp = currentDateTime;
					history.users = user;
					history.versionnumber = versionNumber;
					history.taskhistoryid = Guid.NewGuid();
					context.AddTotaskhistory(history);

					taskOriginal.taskstatuses = context.taskstatuses.FirstOrDefault(c => c.taskstatusid == task.taskStatusId);
				}
							

				if (!IsEqual(task.categoryId,taskOriginal.taskcategories != null ? taskOriginal.taskcategories.id :(int?)null))
				{
					taskhistory history = new taskhistory();
					history.modifiedcolumn = (int)enTaskColumns.eCATEGORY;
					history.newvalue = task.categoryId.HasValue ? task.categoryId.Value.ToString() : null;
					history.oldvalue = taskOriginal.taskcategories != null ? taskOriginal.taskcategories.id.ToString() : null;
					history.tasks = taskOriginal;
					history.timestamp = currentDateTime;
					history.users = user;
					history.versionnumber = versionNumber;
					history.taskhistoryid = Guid.NewGuid();
					context.AddTotaskhistory(history);

					
					taskOriginal.taskcategories = newCategories;
				}
				taskOriginal.versionnumber = versionNumber;
				taskOriginal.LastUpdatedUser = user;
				taskOriginal.lastupdatedatetime = currentDateTime;

				if (comment != null && comment != "")
				{
					taskcomments comments = new taskcomments();
					comments.comments = comment;
					comments.tasks = taskOriginal;
					comments.versionnumber = versionNumber;

					context.AddTotaskcomments(comments);

					taskhistory history = new taskhistory();
					history.modifiedcolumn = (int)enTaskColumns.eCOMMENTS;
					history.newvalue = null;
					history.oldvalue =  null;
					history.tasks = taskOriginal;
					history.timestamp = currentDateTime;
					history.users = user;
					history.versionnumber = versionNumber;
					history.taskhistoryid = Guid.NewGuid();
					context.AddTotaskhistory(history);

				}
				context.SaveChanges();
				return (TaskDTO)taskOriginal;
			}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing GetTaskCategoryList  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}
	public byte[] GetTaskHistory(Guid taskid)
	{
		string sText = "<html><body>";
		using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
		{
			var historyList = (from d in(from i in context.taskhistory.Include("tasks").Include("users") where i.tasks.taskid == taskid
							  select new { taskid = i.tasks.taskid, oldValue = i.oldvalue, newValue = i.newvalue, 
								  username = i.users.FirstName + " " + i.users.LastName, 
								  timeStamp = i.timestamp, versionNumber = i.versionnumber,column = i.modifiedcolumn })

			group d by new
																	   {
																		   d.versionNumber,
																		   d.timeStamp,
																		   d.username,
																		   d.newValue,
																		   d.oldValue,
																		   d.taskid,
																		   d.column
																	   } into dg
							   orderby dg.Key.versionNumber descending
							   select new
							   {
								   taskid = dg.Key.taskid,
								   oldValue = dg.Key.oldValue,
								   newValue = dg.Key.newValue, 
								  username = dg.Key.username,
								   timeStamp = dg.Key.timeStamp,
								   versionNumber = dg.Key.versionNumber,
								   column = dg.Key.column
							   });
			int versionNumber = 0,index =0;
			
			foreach (var item in historyList)
			{
				if (versionNumber != item.versionNumber)
				{
					if (versionNumber != 0)
					{
						sText += "</ul>";
						taskcomments comments = context.taskcomments.Include("tasks").FirstOrDefault(t => t.versionnumber == versionNumber && t.tasks.taskid == item.taskid);
						if (comments != null)
							sText += "<p>" + comments.comments+"</p>";
						sText += "<hr/>";

					}
					sText += "<p><b>#" + item.versionNumber + " Updated by " + item.username + " on " + item.timeStamp.ToString() +"</b></p>";
					//insert line 
				
					versionNumber = item.versionNumber.Value;
					sText += "<ul>";
				}
				string sChange = GetHistoryChangeString(item.newValue, item.oldValue, item.column.Value, context);
				if(sChange.Length !=0)
					sText += "<li>" + sChange + "</li>";

				if (index == historyList.Count() - 1)
				{
					sText += "</ul>";
					taskcomments comments = context.taskcomments.Include("tasks").FirstOrDefault(t => t.versionnumber == versionNumber && t.tasks.taskid == item.taskid);
					if (comments != null)
						sText += "<p>" + comments.comments + "</p>";
					sText += "<hr/>";
				}
				index++;
				

			}
		}
		sText += "</body></html>";
		return ConvertToByte(sText, sText.Length);
	}

	public void DeleteTask(Guid taskId)
	{
		int iTotalStartTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
		try
		{
			using (iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>())
			{

				tasks task = context.tasks.FirstOrDefault<tasks>(a => a.taskid == taskId);
				if (task == null)
					throw new Exception("task not found");


				context.DeleteObject(task);

				context.SaveChanges();
			}
		}
		catch (Exception exc)
		{
			log.Error(exc);
			throw;
		}
		finally
		{
			int iTotalEndTime = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
			log.InfoFormat("finished processing DeleteTask  in {0} seconds", (iTotalEndTime - iTotalStartTime));
		}
	}
	private byte[] ConvertToByte(string theString, int desiredLength)
	{
		ASCIIEncoding encoder = new ASCIIEncoding();
		byte[] byteStream = new byte[(uint)desiredLength];
		encoder.GetBytes(theString, 0, (theString.Length <= desiredLength) ? theString.Length : desiredLength, byteStream, 0);
		return byteStream;
	} 

	private string GetHistoryChangeString(string newVal, string oldVal, int coulmn, iZINEEntities context)
	{
		string sText = String.Empty;
		Guid gNewId ,gOldId;
		int iNewId,iOldId;
		switch(coulmn)
		{
			case (int)enTaskColumns.eASSIGNTO :
				string newUserName = "blank",oldUserName = "blank";
				if(newVal != null && newVal != "")
				{
					gNewId = new Guid(newVal);
					User newuser = context.Users.FirstOrDefault(u => u.UserId == gNewId);
					newUserName = newuser.FirstName + " " + newuser.LastName;
				}
				if(oldVal != null && oldVal != "")
				{
					gOldId = new Guid(oldVal);
					User olduser = context.Users.FirstOrDefault(u => u.UserId == gOldId);
					oldUserName = olduser.FirstName + " " + olduser.LastName;
				}
				
				if (oldVal == null || oldVal == "")
				{
					sText = "<b>AssignTo</b> set to <i>" + newUserName +"</i>"; 
				}
				else
				{

					sText = "<b>AssignTo</b> change from <i>" + oldUserName + "</i> to <i>" + newUserName+"</i>";
				}
				break;
			case (int)enTaskColumns.eCATEGORY :
				string newCatName = "blank", oldCatName = "blank";
				if (newVal != null && newVal != "")
				{
					iNewId = int.Parse(newVal);
					taskcategories newcategory = context.taskcategories.FirstOrDefault(u => u.id == iNewId);
					newCatName = newcategory.name;
				}
				if (oldVal != null && oldVal != "" && oldVal != "0")
				{
					iOldId = int.Parse(oldVal);
					taskcategories oldCategory = context.taskcategories.FirstOrDefault(u => u.id == iOldId);
					oldCatName = oldCategory.name;
				}

				if (oldVal == null || oldVal == "" || oldVal == "0")
				{
					sText = "<b>Category</b> set to <i>" + newCatName +"</i>";
				}
				else
				{

					sText = "<b>Category</b> change from <i>" + oldCatName + "</i> to <i>" + newCatName +"</i>";
				}
				break;
				case (int)enTaskColumns.eDESCRIPTION :
				if (oldVal == null || oldVal == "")
				{
					sText = "<b>Description</b> set to <i>" + newVal +"</i>";
				}
				else
				{

					sText = "<b>Description</b> change from <i>" + oldVal + "</i> to " + newVal +"</i>";
				}
				break;
			case (int)enTaskColumns.eDOCUMENTID :
				string newAssetName = "blank", oldAssetName = "blank";
				if (newVal != null && newVal != "")
				{
					gNewId = new Guid(newVal);
					Asset newAsset = context.Assets.FirstOrDefault(u => u.AssetId == gNewId);
					newAssetName = newAsset.Name;
				}
				if (oldVal != null && oldVal != "")
				{
					gOldId = new Guid(oldVal);
					Asset oldAsset = context.Assets.FirstOrDefault(u => u.AssetId == gOldId);
					oldAssetName = oldAsset.Name;
				}

				if (oldVal == null || oldVal == "")
				{
					sText = "<b>Document</b> set to <i>" + newAssetName +"</i>";
				}
				else
				{

					sText = "<b>Document</b> change from <i>" + oldAssetName + "</i> to <i>" + newAssetName+"</i>";
				}
				break;
				case (int)enTaskColumns.eEDITIONID :
				break;
			case (int)enTaskColumns.ePAGE:
				if (oldVal == null || oldVal == "")
				{
					sText = "<b>Page</b> set to <i>" + newVal +"</i>";
				}
				else
				{

					sText = "<b>Page</b> change from <i>" + oldVal + "</i> to <i>" + newVal+"</i>";
				}
				break;
				case (int)enTaskColumns.eSPREAD :
				if (oldVal == null || oldVal == "")
				{
					sText = "<b>Spread</b> set to <i>" + newVal +"</i>";
				}
				else
				{

					sText = "<b>Spread</b> change from <i>" + oldVal + "</i> to <i>" + newVal + "</i>";
				}
				break;
			case (int)enTaskColumns.eSTAUSID:
				string newStatusName = "blank", oldStatusName = "blank";
				if (newVal != null && newVal != "")
				{
					iNewId = int.Parse(newVal);
					taskstatuses newStatus = context.taskstatuses.FirstOrDefault(u => u.taskstatusid == iNewId);
					newStatusName = newStatus.name;
				}
				if (oldVal != null && oldVal != "")
				{
					iOldId = int.Parse(oldVal);
					taskstatuses oldStatus = context.taskstatuses.FirstOrDefault(u => u.taskstatusid == iOldId);
					oldStatusName = oldStatus.name;
				}

				if (oldVal == null || oldVal == "")
				{
					sText = "<b>Status</b> set to <i>" + newStatusName +"</i>";
				}
				else
				{

					sText = "<b>Status</b> change from <i>" + oldStatusName + "</i> to <i>" + newStatusName +"</i>";
				}
				break;
			case (int)enTaskColumns.eSUBJECT :
				if (oldVal == null || oldVal == "")
				{
					sText = "<b>Subject</b> set to <i>" + newVal +"</i>";
				}
				else
				{

					sText = "<b>Subject</b> change from <i>" + oldVal + "</i> to <i>" + newVal +"</i>";
				}
				break;
			
		}
	
		return sText;
	}
	private bool IsEqual(object first, object second)
	{
		if (first == null)
			first = "";
		if (second == null)
			second = "";

		if (first.ToString().Trim().ToLower() != second.ToString().Trim().ToLower())
			return false;
		return true;
	}

}
