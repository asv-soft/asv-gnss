namespace Asv.Gnss
{
    public class UbxConfigurationLoadCmd : UbxConfigurationBaseCmd
    {
        protected override ConfAction Action => ConfAction.Load;
        public override string Name => base.Name + "-ConfigurationLoad";
    }
}