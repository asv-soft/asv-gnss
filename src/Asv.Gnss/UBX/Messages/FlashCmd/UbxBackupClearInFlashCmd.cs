namespace Asv.Gnss
{
    public class UbxBackupClearInFlashCmd : UbxBackupInFlashBaseCmd
    {
        protected override byte Command => 1;
        public override string Name => base.Name + "-ClearBackupInFlash";
    }
}