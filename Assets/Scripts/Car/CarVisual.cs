using Photon.Pun;
using UnityEngine;

public class CarVisual : MonoBehaviourPun
{
    [Tooltip("List of Mesh Renderers to which the new material will be applied")]
    [SerializeField] private MeshRenderer[] meshRenderers;

    public void ApplyColor(Material material)
    {
        if (meshRenderers.Length <= 0) return;

        ApplyMaterialToAllSlots(material);

        if (PhotonNetwork.IsConnected && photonView.IsMine)
            photonView.RPC(nameof(SyncCarColor), RpcTarget.OthersBuffered, material.name);
    }

    private void ApplyMaterialToAllSlots(Material material)
    {
        foreach (var meshRenderer in meshRenderers)
        {
            if (meshRenderer == null) continue;

            Material[] mats = meshRenderer.materials;
            mats[0] = material;

            meshRenderer.materials = mats;
        }
    }

    [PunRPC]
    public void SyncCarColor(string materialName)
    {
        Material material = Resources.Load<Material>($"Materials/{materialName}");
            if (material == null) return;
            ApplyMaterialToAllSlots(material);
    }
}
