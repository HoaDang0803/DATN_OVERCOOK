using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    private const string WALKING_ANIMATION = "IsWalking";
    private Animator animator;
    [SerializeField] private Player player;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!IsOwner) return;
        animator.SetBool(WALKING_ANIMATION, player.IsWalking());
    }
}
