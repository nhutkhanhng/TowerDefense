using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour {

	public Color hoverColor;
	public Color notEnoughMoneyColor;
    public Vector3 positionOffset;


    [SerializeField]
    public LayerMask settingLayerMask;

    [HideInInspector]
	public GameObject turret;


    [HideInInspector]
	public TurretBlueprint turretBlueprint;
	[HideInInspector]
	public bool isUpgraded = false;

	private Renderer rend;
	private Color startColor;
    
	BuildManager buildManager;

	void Start ()
	{
        // this.tag = "Player";

		rend = GetComponent<Renderer>();
		startColor = rend.material.color;

        if ( turret == null)
            settingLayerMask.value = LayerMask.NameToLayer("Ground");
        else
            settingLayerMask.value = LayerMask.NameToLayer("Unwalkable");

        buildManager = BuildManager.instance;
    }


    public Vector3 GetBuildPosition ()
	{
		return transform.position + positionOffset;
	}

	void OnMouseDown ()
	{
		if (EventSystem.current.IsPointerOverGameObject())
			return;

        if (turret != null)
		{
			buildManager.SelectNode(this);
			return;
		}

        if (!buildManager.CanBuild)
            return;

        BuildTurret(buildManager.GetTurretToBuild());

        Debug.Log("Mouse Down");
    }

	void BuildTurret (TurretBlueprint blueprint)
	{
		if (PlayerStats.Money < blueprint.cost)
		{
			Debug.Log("Not enough money to build that!");
			return;
		}


        settingLayerMask.value = LayerMask.NameToLayer("Unwalkable");

        gameObject.layer = settingLayerMask;

        Grid.instance.NodeFromWorldPoint(transform.position).walkable = false;

        if (!isBuildPath.instance.checkPath())
        {
            settingLayerMask.value = LayerMask.NameToLayer("Ground");

            gameObject.layer = settingLayerMask;

            Grid.instance.NodeFromWorldPoint(transform.position).walkable = true;

            rend.material.color = Color.red;

            return;
        }

        PlayerStats.Money -= blueprint.cost;

        GameObject _turret = (GameObject)Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        turretBlueprint = blueprint;

        GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
		Destroy(effect, 5f);

        Debug.Log("Turret build!");
    }

    public void UpgradeTurret ()
	{
		if (PlayerStats.Money < turretBlueprint.upgradeCost)
		{
			Debug.Log("Not enough money to upgrade that!");
			return;
		}

		PlayerStats.Money -= turretBlueprint.upgradeCost;

		//Get rid of the old turret
		Destroy(turret);

		//Build a new one
		GameObject _turret = (GameObject)Instantiate(turretBlueprint.upgradedPrefab, GetBuildPosition(), Quaternion.identity);
		turret = _turret;

		GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
		Destroy(effect, 5f);

		isUpgraded = true;

        Debug.Log("Turret upgraded!");
	}

	public void SellTurret ()
	{
		PlayerStats.Money += turretBlueprint.GetSellAmount();

		GameObject effect = (GameObject)Instantiate(buildManager.sellEffect, GetBuildPosition(), Quaternion.identity);
		Destroy(effect, 5f);

		Destroy(turret);
		turretBlueprint = null;

        settingLayerMask.value = LayerMask.NameToLayer("Ground");

        Grid.instance.NodeFromWorldPoint(transform.position).walkable = true;
        gameObject.layer = settingLayerMask;
    }

	void OnMouseEnter ()
	{
		if (EventSystem.current.IsPointerOverGameObject())
			return;

        if (!buildManager.CanBuild || turretBlueprint == null)
            return;

        if (buildManager.CanBuild)
		{
            if (buildManager.HasMoney)
                rend.material.color = hoverColor;   
            else
                rend.material.color = notEnoughMoneyColor;
		}
        else
		{
            rend.material.color = Color.cyan;
		}

	}

	void OnMouseExit ()
	{
		rend.material.color = startColor;
    }

}
