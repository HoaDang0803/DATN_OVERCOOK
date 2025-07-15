using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private RecipeListSO recipeListSO;
    private List<RecipeSO> waitingRecipeSOList;

    private float spawnRecipeTimer = 4f;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipeMax = 4;
    private int successfulRecipeAmount;

    private void Awake()
    {
        Instance = this;

        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeMax)
            {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);

                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);

            }

        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];

        waitingRecipeSOList.Add(waitingRecipeSO);

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    bool ingridientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            ingridientFound = true;
                            break;
                        }
                    }

                    if (!ingridientFound)
                    {
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe)
                {
                    //Debug.Log("Player delivered the correct recipe!");

                    DeliverCorrectRecipeServerRpc(i);
                    return;
                }
            }
        }

        //Debug.Log("Player didn't deliver a correct recipe!");
        DeliverIncorrectRecipeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex)
    {
        successfulRecipeAmount++;
        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);

        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipeAmount()
    {
        return successfulRecipeAmount;
    }
}
