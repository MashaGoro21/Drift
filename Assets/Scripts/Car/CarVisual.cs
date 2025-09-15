using Photon.Pun;
using UnityEngine;

public class CarVisual : MonoBehaviourPun
{
    [Tooltip("List of Mesh Renderers to which the new material will be applied")]
    [SerializeField] private MeshRenderer[] meshRenderers;

    public void ApplyColor(Material material)
    {
        if (meshRenderers.Length <= 0) return;
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
        
        foreach(var meshRenderer in meshRenderers)
        {
            if (meshRenderer == null) continue;

            Material[] mats = meshRenderer.materials;
            mats[0] = material;
            meshRenderer.materials = mats;

            if(PhotonNetwork.IsConnected && photonView.IsMine)
                photonView.RPC(nameof(SyncCarColor), RpcTarget.Others, material.name);
        }
    }

    [PunRPC]
    public void SyncCarColor(string materialName)
    {
        Material material = Resources.Load<Material>($"Materials/{materialName}");
        foreach (var meshRenderer in meshRenderers)
        {
            if (meshRenderer == null) continue;
            Material[] mats = meshRenderer.materials;
            mats[0] = material;
            meshRenderer.materials = mats;
        }
    }
}
