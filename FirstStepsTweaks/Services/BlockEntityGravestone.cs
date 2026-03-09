using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace FirstStepsTweaks.Services
{
    public class BlockEntityGravestone : BlockEntity
    {
        private const string GraveIdAttr = "graveId";
        private const string OwnerUidAttr = "ownerUid";

        public string GraveId { get; private set; } = string.Empty;
        public string OwnerUid { get; private set; } = string.Empty;

        public void SetData(string graveId, string ownerUid)
        {
            GraveId = graveId ?? string.Empty;
            OwnerUid = ownerUid ?? string.Empty;
            MarkDirty(true);
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetString(GraveIdAttr, GraveId ?? string.Empty);
            tree.SetString(OwnerUidAttr, OwnerUid ?? string.Empty);
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessForResolve);
            GraveId = tree.GetString(GraveIdAttr, string.Empty);
            OwnerUid = tree.GetString(OwnerUidAttr, string.Empty);
        }
    }
}
