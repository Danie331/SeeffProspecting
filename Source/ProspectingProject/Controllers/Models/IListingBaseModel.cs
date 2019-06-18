
namespace ProspectingProject.Controllers.Models
{
    public interface IListingBaseModel
    {
        int LightstoneId { get; }
        string ToJsonString();
    }
}
