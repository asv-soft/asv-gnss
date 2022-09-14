namespace Asv.Gnss
{
    public class UbxBackupCreateInFlashCmd : UbxBackupInFlashBaseCmd
    {
        protected override byte Command => 0;
        public override string Name => base.Name + "-CreateBackupInFlash";
    }
}