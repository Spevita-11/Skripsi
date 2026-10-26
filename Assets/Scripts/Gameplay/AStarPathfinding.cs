using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    public float speed = 5f; // Kecepatan gerakan
    public GameObject visualize;

    [Header("Joystick Settings")]
    public FixedJoystick joystick; // Referensi ke Fixed Joystick

    private GridPathManager gridManager;
    private List<Vector3> path;
    private int currentWaypoint = 0;
    private CharacterController characterController;

    void Start()
    {
        gridManager = FindAnyObjectByType<GridPathManager>();
        characterController = GetComponent<CharacterController>();

        // Otomatis mencari Fixed Joystick di dalam scene jika belum terhubung di Inspector
        if (joystick == null)
        {
            joystick = FindAnyObjectByType<FixedJoystick>();
        }
    }

    private void Update()
    {
        // 1. JIKA JOYSTICK DIGERAKKAN: Gunakan kontrol joystick secara real-time
        if (joystick != null && joystick.gameObject.activeInHierarchy && (Mathf.Abs(joystick.Horizontal) > 0.05f || Mathf.Abs(joystick.Vertical) > 0.05f))
        {
            // Hentikan coroutine pathfinding agar tidak bentrok
            StopAllCoroutines();
            path = null;

            Vector3 direction = new Vector3(joystick.Horizontal, 0f, joystick.Vertical).normalized;

            // Putar badan karakter menghadap arah joystick
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
            }

            // Gerakkan karakter (mendukung CharacterController atau transform biasa)
            if (characterController != null)
            {
                characterController.Move(direction * speed * Time.deltaTime);
            }
            else
            {
                transform.Translate(direction * speed * Time.deltaTime, Space.World);
            }
        }
        else
        {
            // 2. KODE ASLI ANDA: Klik Mouse / Tap untuk A* Pathfinding
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Posisi klik di tanah: " + hit.point);
                    Vector3 target = hit.point;
                    target.y = transform.position.y;
                    path = FindPath(transform.position, target);
                    currentWaypoint = 0;
                    MoveAlongPath();

                    OnSpawnArrow(target);
                }
            }
        }
    }

    public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = gridManager.NodeFromWorldPoint(startPos);
        Node targetNode = gridManager.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in gridManager.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                float newMovementCostToNeighbor = currentNode.gCost + Vector3.Distance(currentNode.worldPosition, neighbor.worldPosition);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = Vector3.Distance(neighbor.worldPosition, targetNode.worldPosition);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    public void MoveAlongPath()
    {
        StopAllCoroutines();    
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        if(path == null)
            yield break;    

        while (currentWaypoint < path.Count)
        {
            Vector3 targetPosition = path[currentWaypoint];
            targetPosition.y = transform.position.y;
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                transform.LookAt(targetPosition);
                yield return null;
            }

            currentWaypoint++;
        }
    }

    private void OnSpawnArrow(Vector3 target)
    {
        if (!visualize) return;
        GameObject arrow = Instantiate(visualize, target, Quaternion.identity);
        Destroy(arrow, 2f);
    }
}