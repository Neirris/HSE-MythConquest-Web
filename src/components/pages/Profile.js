import React, { Component } from "react";
import "../../styles/pageStyles/Profile.css";
import axios from "axios";
import Swal from "sweetalert2";
import Cookies from "js-cookie";

import ProfileStats from "../pageComponents/ProfileStats";
import ProfileInfo from "../pageComponents/ProfileInfo";

export class Profile extends Component {
  static displayName = Profile.name;

  state = {
    profileData: null,
    profileStatsData: null,
    imageUrl: "",
    isEditing: false,
  };

  async componentDidMount() {
    Cookies.remove('arenaLog');
    Cookies.remove('battleLog');
    await this.getProfileInfo();
    await this.getProfileStats();
  }

  async getProfileInfo() {
    const response = await fetch("profileinfo");
    const data = await response.json();
    this.setState({ profileData: data });
  }

  async getProfileStats() {
    const response = await fetch("profilestats");
    const data = await response.json();
    this.setState({ profileStatsData: data });
  }

  handleImageClick = () => {
    this.setState({ isEditing: true });
  };

  handleImageUpdate = (event) => {
    if (event.key === 'Enter') {
      const { value } = event.target;
      if (this.isValidImageUrl(value)) {
        this.setState({ imageUrl: value, isEditing: false });
        axios.defaults.headers.common["newPfp"] = encodeURI(value);
        axios.post("pfpupdate");
      } else {
        Swal.fire("", "Некорректная ссылка на изображение!", "warning");
        setTimeout(() => {
          window.location.reload();
        }, 1000);
      }
    }
  };

  isValidImageUrl = (url) => {
    const imagePattern = /\.(jpeg|jpg|gif|png|svg)$/;
    return imagePattern.test(url);
  };

  render() {
    const { profileData, profileStatsData, imageUrl, isEditing } = this.state;
    return (
      <div>
        <div className="top" style={{ borderBottom: "3px solid #7289da" }}>
          <div className="left-base">
            <div>
              {isEditing ? (
                <input
                  type="text"
                  value={imageUrl}
                  onChange={(event) =>
                    this.setState({ imageUrl: event.target.value })
                  }
                  onKeyDown={this.handleImageUpdate}
                />
              ) : (
                <img
                  className="profile-image-user"
                  src={imageUrl || (profileData && profileData.user_pfp)}
                  alt=""
                  onClick={this.handleImageClick}
                />
              )}
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
              <img
                className="profile-image"
                src={profileStatsData && profileStatsData.appearance_url}
                alt=""
              />
              <div className="row-c">
                <div className=" text-block">
                  <span className="text-bold">
                    {" "}
                    {profileStatsData && profileStatsData.name}
                  </span>
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
