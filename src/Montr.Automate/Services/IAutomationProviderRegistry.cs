namespace Montr.Automate.Services
{
	public interface IAutomationProviderRegistry
	{
		IAutomationConditionProvider GetConditionProvider(string typeCode);

		IAutomationActionProvider GetActionProvider(string typeCode);
	}
}
