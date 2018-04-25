namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.MyItems
{
  using Sitecore.Configuration;
  using Sitecore.ContentSearch;
  using Sitecore.ContentSearch.Security;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.ExperienceEditor;
  using Sitecore.ExperienceEditor.Speak.Server.Contexts;
  using Sitecore.ExperienceEditor.Speak.Server.Requests;
  using Sitecore.ExperienceEditor.Speak.Server.Responses;
  using Sitecore.ExperienceEditor.Utils;

  public class MyItemsCountRequest : PipelineProcessorRequest<ItemContext>
  {
    public override PipelineProcessorResponseValue ProcessRequest()
    {
      Assert.IsNotNull(base.RequestContext.Database, string.Format("Could not get context.Database for requestArgs:{0}", base.Args.Data));
      Assert.IsNotNull(Factory.GetDatabase(base.RequestContext.Database), string.Format("Could not get database, with name:{0}", base.RequestContext.Database));
      if (!Sitecore.ExperienceEditor.Settings.WebEdit.ShowNumberOfLockedItemsOnButton)
      {
        return new PipelineProcessorResponseValue
        {
          Value = string.Empty
        };
      }
      this.RefreshIndex(base.RequestContext.Item);
      return new PipelineProcessorResponseValue
      {
        Value = ItemUtility.GetLockedItemsCountOfCurrentUser()
      };
    }

    protected virtual void RefreshIndex(Item item)
    {
      using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_master_index").CreateSearchContext(SearchSecurityOptions.Default))
      {
        providerSearchContext.Index.Refresh(new SitecoreIndexableItem(item), IndexingOptions.ForcedIndexing);
      }
    }
  }

}