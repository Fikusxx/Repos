using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{

    [SerializeField] private SceneName teleportToScene; // what scene tp player to
    [SerializeField] private Vector3 positionToTeleport; // what pos tp player to


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();

        if (player != null)
        {
            // Calc players new pos
            float xPosition = Mathf.Approximately(positionToTeleport.x, 0f) ? player.transform.position.x : positionToTeleport.x;
            float yPosition = Mathf.Approximately(positionToTeleport.y, 0f) ? player.transform.position.y : positionToTeleport.y;
            float zPosition = 0f;


            // Teleport the player
            SceneControllerManager.Instance.FadeAndLoadScene(teleportToScene.ToString(), new Vector3(xPosition, yPosition, zPosition));
        }
    }
}
