namespace Asv.Gnss
{
    public class UbxConfigurationSaveCmd : UbxConfigurationBaseCmd
    {
        protected override ConfAction Action => ConfAction.Save;
        public override string Name => base.Name + "-ConfigurationSave";
    }
}