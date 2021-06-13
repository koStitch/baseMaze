using System;
using System.Collections.Generic;
using UnityEngine;
using GM = Completed.GameManager;

public class PathfindingDebugStepVisual : MonoBehaviour
{
    private List<GridSnapshotAction> gridSnapshotActionList;
    private bool autoShowSnapshots;
    private float autoShowSnapshotsTimer;

    private void Awake()
    {
        gridSnapshotActionList = new List<GridSnapshotAction>();
    }

    private void Update()
    {
        if (autoShowSnapshots)
        {
            float autoShowSnapshotsTimerMax = .05f;
            autoShowSnapshotsTimer -= Time.deltaTime;
            if (autoShowSnapshotsTimer <= 0f)
            {
                autoShowSnapshotsTimer += autoShowSnapshotsTimerMax;
                ShowNextSnapshot();
                if (gridSnapshotActionList.Count == 0)
                {
                    autoShowSnapshots = false;
                }
            }
        }
    }

    public bool IsShowingSnapshots()
    {
        return autoShowSnapshots;
    }

    public void ShowSnapshots()
    {
        autoShowSnapshots = true;
        GM.instance.pathfindingDebugCounter++;
    }

    private void ShowNextSnapshot()
    {
        if (gridSnapshotActionList.Count > 0)
        {
            GridSnapshotAction gridSnapshotAction = gridSnapshotActionList[0];
            gridSnapshotActionList.RemoveAt(0);
            gridSnapshotAction.TriggerAction();
        }
    }

    public void ClearSnapshots()
    {
        gridSnapshotActionList.Clear();
    }

    public void TakeSnapshot(Grid<PathNode> grid, PathNode current, List<PathNode> openList, List<PathNode> closedList)
    {
        GridSnapshotAction gridSnapshotAction = new GridSnapshotAction();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);

                int gCost = pathNode.gCost;
                int hCost = pathNode.hCost;
                int fCost = pathNode.fCost;
                bool isCurrent = pathNode == current;
                bool isInOpenList = openList.Contains(pathNode);
                bool isInClosedList = closedList.Contains(pathNode);
                int tmpX = x;
                int tmpY = y;

                gridSnapshotAction.AddAction(() =>
                {
                    Transform visualNode = GM.instance.GetBoardScript().VisualNodesList[GM.instance.pathfindingDebugCounter][tmpX, tmpY];

                    visualNode.gameObject.SetActive(true);

                    Color backgroundColor = visualNode.GetComponent<SpriteRenderer>().color;

                    if (isInClosedList)
                    {
                        backgroundColor = new Color(1, 0, 0);
                    }

                    if (isInOpenList)
                    {
                        backgroundColor = Color.grey;
                    }

                    if (isCurrent)
                    {
                        backgroundColor = new Color(0, 1, 0);
                    }


                    visualNode.GetComponent<SpriteRenderer>().color = backgroundColor;
                });
            }
        }

        gridSnapshotActionList.Add(gridSnapshotAction);
    }

    public void TakeSnapshotFinalPath(Grid<PathNode> grid, List<PathNode> path)
    {
        GridSnapshotAction gridSnapshotAction = new GridSnapshotAction();
        Color backgroundColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f, 0.3f, 0.3f);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);

                int gCost = pathNode.gCost;
                int hCost = pathNode.hCost;
                int fCost = pathNode.fCost;
                bool isInPath = path.Contains(pathNode);
                int tmpX = x;
                int tmpY = y;

                gridSnapshotAction.AddAction(() =>
                {
                    Transform visualNode = GM.instance.GetBoardScript().VisualNodesList[GM.instance.pathfindingDebugCounter][tmpX, tmpY];

                    if (isInPath)
                    {
                        var finalNodeHolderName = "FinalNodeHolder" + GM.instance.pathfindingDebugCounter.ToString();
                        var finalNodeHolder = GameObject.Find(finalNodeHolderName).transform;
                        visualNode.transform.SetParent(finalNodeHolder);
                    }
                    else
                    {
                        visualNode.gameObject.SetActive(false);
                    }

                    visualNode.GetComponent<SpriteRenderer>().color = backgroundColor;
                });
            }
        }

        gridSnapshotActionList.Add(gridSnapshotAction);
    }

    private class GridSnapshotAction
    {
        private Action action;

        public GridSnapshotAction()
        {
            action = () => { };
        }

        public void AddAction(Action action)
        {
            this.action += action;
        }

        public void TriggerAction()
        {
            action();
        }
    }
}