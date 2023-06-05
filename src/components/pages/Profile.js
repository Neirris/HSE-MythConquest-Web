import React, { Component } from "react";
import "../../styles/pageStyles/Profile.css";

import ProfileStats from "../pageComponents/ProfileStats";
import ProfileInfo from "../pageComponents/ProfileInfo";

export class Profile extends Component {
  static displayName = Profile.name;

  state = {
    profileData: null,
    profileStatsData: null,
  };

  async componentDidMount() {
        await this.getProfileInfo();
        await this.getProfileStats();
  }

  async getProfileInfo() {
    const response = await fetch('profileinfo');
    const data = await response.json();
    this.setState({ profileData: data });
  }

  async getProfileStats() {
    const response = await fetch('profilestats');
    const data = await response.json();
    this.setState({ profileStatsData: data });
  }
  

  render() {
    const { profileData, profileStatsData } = this.state;
    return (
      <div>
        <div className="top" style={{ borderBottom: "3px solid #7289da" }}>
          <div className="left-base">
            <div>
              <img className="profile-image-user" src={profileData && (profileData.user_pfp)} alt="" />
            </div>
          </div>
          <div className="right-base">
          {profileData && (
          <ProfileInfo
            nickName={profileData.nickname}
            lvl={profileData.lvl}
            exp={profileData.exp}
            expReq={profileData.exp_req}
            money={profileData.coins}
            valueSum="XXX"
            pvpWin={profileData.wins}
            pvpLose={profileData.loses}
            towerLvl="XXX"
          />
        )}
          </div>
        </div>
        <div className="bottom">
          <div className="left-base">
            <div className="column">
              <img className="profile-image" src={profileStatsData && (profileStatsData.appearance_url)} alt="" />
              <div className="row-c">
                <div className=" text-block">
                  <span className="text-bold"> {profileStatsData && (profileStatsData.name)}</span>          
                </div>
              </div>
            </div>
          </div>
          <div className="right-base">
          {profileStatsData && (
            <ProfileStats
              className={profileStatsData.name}
              rarity={profileStatsData.rarity}
              gender={profileStatsData.gender}
              value={profileStatsData.value}
              attack={profileStatsData.attack}
              crit={profileStatsData.crit}
              initiative={profileStatsData.initiative}
              evasion={profileStatsData.evasion}
              health={profileStatsData.health_points}
              defense={profileStatsData.defense}
            />
            )}
          </div>
        </div>
      </div>
    );
  }
}
