namespace Asv.Gnss
{
    public class UbxConfigurationClearCmd : UbxConfigurationBaseCmd
    {
        protected override ConfAction Action => ConfAction.Clear;
        public override string Name => base.Name + "-ConfigurationClear";
    }
}