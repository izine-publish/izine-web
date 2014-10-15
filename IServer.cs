using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using System.Net.Security;
using System.ServiceModel.Activation;
using iZINE.Web.Server;
using iZINE.WcfAnnotation;


/*
SOAP-UI:
When having wsHttpBinding, disable transaction. Enable WS-A addressing, and ensure action and to are correctly set.
 */
[MessageContract(WrapperNamespace="http://izine-publish.net/server/")]
public class UploadMessage
{
    [MessageHeader(Namespace = "http://izine-publish.net/server/", Name = "assetid")]
    public Guid assetid;
    
    [MessageBodyMember(Namespace="http://izine-publish.net/server/", Name="data")]
    public Stream data;
    
}
/*
[MessageContract(WrapperNamespace = "http://izine-publish.net/server/")]
public class UploadPage
{
    [MessageHeader(Namespace = "http://izine-publish.net/server/", Name = "pageid")]
    public Guid pageid;

    [MessageHeader(Namespace = "http://izine-publish.net/server/", Name = "assetid")]
    public Guid assetid;

    [MessageHeader(Namespace = "http://izine-publish.net/server/", Name = "number")]
    public int number;

    [MessageBodyMember(Namespace = "http://izine-publish.net/server/", Name = "data")]
    public Stream data;
}
*/
[ServiceContract(Namespace = "http://izine-publish.net/", Name="server" )]
[XmlSerializerFormat]
public interface IServer
{
    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    [Documentation("login")]
    UserDTO Login();

    // [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    // void Upload(UploadMessage message);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    TitleDTO[] GetTitleList();

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    Guid GetRole();

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    CommentDTO[] GetCommentList(Guid assetid);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    String[] GetTags(Guid assetid);

    //[OperationContract(ProtectionLevel = ProtectionLevel.None)]
    //AssetDTO[] GetAssetList_Old(Guid shelveid, Guid[] types);
    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    void UpdateAssetStatus(Guid assetid, Guid oldstatusid, Guid newstatusid);
    

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    ShelveDTO[] GetShelveList(Guid titleid);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    bool Logoff();

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    UserDTO GetUser();

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    byte[] GetThumbnail(Guid assetid);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    LockDTO LockAsset(Guid assetid, LockDTO lockDTO);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    bool UnlockAsset(Guid assetid, bool forceUnlock);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    void DeleteAsset(Guid assetid);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
	AssetDTO CreateAsset(AssetDTO assetDTO);
    
    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    void UpdateAssetName(Guid assetid, String name);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    AssetDTO[] GetAsset(Guid[] assetid);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    void UploadPDF(Guid assetid, Guid versionid, byte[] data);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    void UploadPage(Guid pageid, Guid versionid, int number, Guid pagetypeid, byte[] data, bool isCompressed);
    
    [OperationContract(ProtectionLevel = ProtectionLevel.None)]//, Action = "http://izine-publish.net/server/" + "UploadVersion") ]
    void UploadVersion(Guid assetid, VersionDTO versionDto);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    void UnlinkAsset(Guid assetid);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    void RelinkAsset(Guid documentid, Guid assetid);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    TaskDTO[] GetEditionTaskList(Guid editionId);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    void CheckOutAsset(string assetid);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    EditionTitlesDTO[] GetEditionsByTitle(Guid titleid);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    TitleStatusDTO[] GetStatusByTitle(Guid titleid);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    AssetDTO[] GetAssetList(Guid shelveid, Guid[] types);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    void UpdateAsset(Guid assetid, AssetDTO assetdto);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    LockDTO[] GetLock(Guid[] assetids);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
	AssetDTO CheckInAsset(Guid assetid, Guid statusid, String comment, Guid versionId,int headVersion);

    [OperationContract(ProtectionLevel = ProtectionLevel.None)]
    VersionDTO DownloadVersion(Guid versionid);

	[OperationContract(ProtectionLevel = ProtectionLevel.None)]
	UserDTO[] GetUsersForTitle(Guid titleId);

	[OperationContract(ProtectionLevel = ProtectionLevel.None)]
	IdNameDTO[] GetTaskStatusList();

	[OperationContract(ProtectionLevel = ProtectionLevel.None)]
	TaskDTO CreateTask(TaskDTO task);

	[OperationContract(ProtectionLevel = ProtectionLevel.None)]
	IdNameDTO[] GetTaskCategoryList();

	[OperationContract(ProtectionLevel = ProtectionLevel.None)]
	TaskDTO UpdateTask(TaskDTO task,string comment);

	[OperationContract(ProtectionLevel = ProtectionLevel.None)]
	byte[] GetTaskHistory(Guid taskid);

	[OperationContract(ProtectionLevel = ProtectionLevel.None)]
	void DeleteTask(Guid taskId);
}
