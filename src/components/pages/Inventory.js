import React, { Component } from "react";
import "../../styles/pageStyles/Inventory.css";
import InventoryStats from "../pageComponents/InventoryStats";
import "../../styles/utilStyles/BaseStyles.css";

export class Inventory extends Component {
  static displayName = Inventory.name;

  state = {
    profileData: null,
    selectedProfileId: null,
    defaultProfile: {
      name: "XXX",
      className: "XXX",
      rarity: "XXX",
      gender: "XXX",
      value: "XXX",
      attack: "XXX",
      crit: "XXX",
      initiative: "XXX",
      evasion: "XXX",
      health: "XXX",
      defense: "XXX",
    },
  };

  renderImages() {
    const { profileData } = this.state;

    if (!profileData) {
      return null;
    }

    return Object.values(profileData).map((profile) => (
      <img
        key={profile.char_id}
        src={profile.appearance_url}
        className="inventory-image"
        alt=""
        onClick={() => this.handleImageClick(profile.char_id)}
      />
    ));
  }

  async componentDidMount() {
    await this.getInventoryChars();
  }

  async getInventoryChars() {
    const response = await fetch("inventorychars");
    const data = await response.json();
    this.setState({ profileData: data });
  }

  handleImageClick = (profileId) => {
    const selectedProfile = this.state.profileData[profileId];
    this.setState({ selectedProfileId: profileId, selectedProfile });
  };

  render() {
    const { selectedProfileId, profileData, defaultProfile } = this.state;
    const selectedProfile = selectedProfileId
      ? profileData[selectedProfileId]
      : null;

    return (
      <div className="row">
        <div className="left-base" style={{ height: "100vh" }}>
          <div>{this.renderImages()}</div>
        </div>
        <div className="right-base">
          {selectedProfile ? (
            <InventoryStats
              name={selectedProfile.name}
              className={selectedProfile.char_class}
              rarity={selectedProfile.rarity}
              gender={selectedProfile.gender}
              value={selectedProfile.value}
              attack={selectedProfile.attack}
              crit={selectedProfile.crit}
              initiative={selectedProfile.initiative}
              evasion={selectedProfile.evasion}
              health={selectedProfile.health_points}
              defense={selectedProfile.defense}
            />
          ) : (
            <InventoryStats
              name={defaultProfile.name}
              className={defaultProfile.className}
              rarity={defaultProfile.rarity}
              gender={defaultProfile.gender}
              value={defaultProfile.value}
              attack={defaultProfile.attack}
              crit={defaultProfile.crit}
              initiative={defaultProfile.initiative}
              evasion={defaultProfile.evasion}
              health={defaultProfile.health}
              defense={defaultProfile.defense}
            />
          )}
          <div className="column align-center">
            <button className="button-long text-40px">Выбрать персонажа</button>
          </div>
        </div>
      </div>
    );
  }
}
