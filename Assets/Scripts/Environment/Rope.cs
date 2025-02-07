using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField]
    private bool anchored;
    [SerializeField]
    private float spaceBetweenLinks = 0.1f;
    [SerializeField]
    private GameObject attachment;

    private int numOfLinks;
    private Vector3[] linePositions;
    private GameObject start;
    private GameObject end;
    private GameObject linkPrefab;
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        GenerateRope();
    }

    private void Update() {
        for (int i = 0; i <= numOfLinks; i++) {
            if (anchored) {
                lineRenderer.SetPosition(i, gameObject.transform.localPosition + gameObject.transform.GetChild(i+1).transform.localPosition);
            } else {
                lineRenderer.SetPosition(i, gameObject.transform.localPosition + gameObject.transform.GetChild(i).transform.localPosition);
            }
        }
    }

    protected virtual void GenerateRope() {
        start = gameObject.transform.GetChild(0).gameObject;
        end = gameObject.transform.GetChild(1).gameObject;
        linkPrefab = (GameObject)Resources.Load("Prefabs/Rope/Link", typeof(GameObject));
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        float ropeDis = Vector3.Distance(start.transform.localPosition, end.transform.localPosition);
        numOfLinks = Mathf.CeilToInt(ropeDis / spaceBetweenLinks);
        float linkDis = ropeDis/numOfLinks;
        Vector2 ropeVec = new Vector3((end.transform.localPosition.x - start.transform.localPosition.x) / (numOfLinks * linkPrefab.transform.localScale.x), (end.transform.localPosition.y - start.transform.localPosition.y) / (numOfLinks * linkPrefab.transform.localScale.y));

        lineRenderer.positionCount = numOfLinks + 1;
        linePositions = new Vector3[numOfLinks + 1];
        Vector3 parentTrans = gameObject.transform.localPosition;
        lineRenderer.SetPosition(0, parentTrans);
        linePositions[0] = parentTrans;
        
        Destroy(end);
        //Transform baseTransform = new Transform(transform);
        GameObject firstLink = Instantiate(linkPrefab, parentTrans, Quaternion.identity, transform);
        Quaternion linkRot = Quaternion.AngleAxis(Vector3.Angle(Vector3.down, end.transform.localPosition - start.transform.localPosition), Vector3.forward);
        firstLink.transform.GetChild(0).localRotation = linkRot;
        //firstLink.transform.GetChild(0).localRotation = Quaternion.AngleAxis(90, Vector3.forward);
        if (anchored) {
            firstLink.GetComponent<HingeJoint2D>().connectedBody = start.GetComponent<Rigidbody2D>();
        } else {
            Destroy(start);
            Destroy(firstLink.GetComponent<HingeJoint2D>());
        }
        
        Rigidbody2D prevRB = firstLink.GetComponent<Rigidbody2D>();
        Transform lastLink = null;
        for (int i = 1; i <= numOfLinks; i++) {
            GameObject link = Instantiate(linkPrefab, parentTrans + ((linkPrefab.transform.localScale.x * i) * (new Vector3(ropeVec.x, ropeVec.y, 0))), Quaternion.identity, transform);
            link.transform.GetChild(0).localRotation = linkRot;
            HingeJoint2D joint = link.GetComponent<HingeJoint2D>();
            joint.connectedAnchor = ropeVec;
            joint.connectedBody = prevRB;
            prevRB = link.GetComponent<Rigidbody2D>();

            lineRenderer.SetPosition(i, parentTrans + gameObject.transform.GetChild(i + 1).transform.localPosition);
            linePositions[i] = parentTrans + gameObject.transform.GetChild(i + 1).transform.localPosition;
            if (i == numOfLinks) lastLink = link.transform;
        }

        if (attachment != null)
        {
            GameObject link = Instantiate(attachment);
            link.transform.GetChild(0).localRotation = linkRot;
            HingeJoint2D joint = link.AddComponent<HingeJoint2D>();
            joint.connectedAnchor = ropeVec;
            joint.connectedBody = prevRB;
            attachment.transform.SetParent(lastLink, false);
            lineRenderer.SetPosition(numOfLinks + 1, parentTrans + gameObject.transform.GetChild(numOfLinks + 1 + 1).transform.localPosition);
            linePositions[numOfLinks + 1] = parentTrans + gameObject.transform.GetChild(numOfLinks + 1 + 1).transform.localPosition;
        }
    }
}
