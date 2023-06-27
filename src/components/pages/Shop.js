import React, { Component } from "react";
import { createRoot } from "react-dom/client";
import "../../styles/pageStyles/Shop.css";
import axios from "axios";
import CustomAlert from "../utils/CustomAlert";
import Cookies from "js-cookie";
import Swal from "sweetalert2";

export class Shop extends Component {
  static displayName = Shop.name;
  customAlertRoot = null;

  state = {
    tPrice: null,
    CoinsData: null,
    ePrice: null,
    eLvl: null,
  };

  async componentDidMount() {
    Cookies.remove("arenaLog");
    Cookies.remove("battleLog");
    await this.getTicketPrice();
    await this.getCoinsData();
    await this.getEquipmentPrice();
    await this.getEquipmentLvl();
  }

  async getTicketPrice() {
    const response = await fetch("getshopticketprice");
    const data = await response.json();
    this.setState({ tPrice: data });
  }

  async getCoinsData() {
    const response = await fetch("getshopcoins");
    const data = await response.json();
    this.setState({ CoinsData: data });
  }

  async getEquipmentPrice() {
    const response = await fetch("getequipmentprice");
    const data = await response.json();
    this.setState({ ePrice: data });
  }

  async getEquipmentLvl() {
    const response = await fetch("getequipmentlvl");
    const data = await response.json();
    this.setState({ eLvl: data });
  }

  upgradeEquip = (equipmentType) => {
    const { eLvl, CoinsData, ePrice } = this.state;
    const updatedLvl = {
      ...eLvl,
      [equipmentType]: (parseInt(eLvl[equipmentType]) + 1).toString(),
    };

    const updatedCoinsData = {
      ...CoinsData,
      coins: (
        parseInt(CoinsData.coins) -
        parseInt(ePrice[equipmentType.replace("lvl_", "") + "_price"])
      ).toString(),
    };

    this.setState({ eLvl: updatedLvl, CoinsData: updatedCoinsData });
    axios.defaults.headers.common["equipLvl"] = JSON.stringify(updatedLvl);
    axios.defaults.headers.common["coins"] = updatedCoinsData.coins;
    axios.post("equipupgrade").then((response) => {});
  };

  async buyTicket() {
    const { CoinsData } = this.state;
    if (CoinsData && parseInt(CoinsData.coins) <= 1000) {
      Swal.fire("", "Недостаточно монет!", "warning");
      return;
    }
    const updatedCoinsData = {
      ...CoinsData,
      coins: (parseInt(CoinsData.coins) - 1000).toString(), //fix
    };
    axios.defaults.headers.common["ticket"] = (
      parseInt(CoinsData.curr_ticket) + 1
    ).toString();
    axios.defaults.headers.common["coins"] = updatedCoinsData.coins;
    axios.post("buyticket")
    .then((response) => {
      const updatedCoinsData = {
        ...CoinsData,
        curr_ticket: (parseInt(CoinsData.curr_ticket) + 1).toString(),
        coins: (parseInt(CoinsData.coins) - 1000).toString(),
      };
      this.setState({ CoinsData: updatedCoinsData });
    })
  }

  async summonChar() {
    const { CoinsData } = this.state;
    if (CoinsData && parseInt(CoinsData.curr_ticket) === 0) {
      Swal.fire("", "Не хватает билетов призыва!", "warning");
      return;
    }

    if (!this.customAlertRoot) {
      this.customAlertRoot = createRoot(
        document.getElementById("custom-alert-root")
      );
    }

    axios.post("summonchar")
    .then((response) => {
      const result = response.data;
      const { name, appearance_url, itemRarity, ascension_lvl } = result;
      const updatedCoinsData = {
        ...CoinsData,
        curr_ticket: (parseInt(CoinsData.curr_ticket) - 1).toString(),
      };
      this.setState({ CoinsData: updatedCoinsData });
      const onClose = () => {
        window.location.reload();
      };

      this.customAlertRoot.render(
        <CustomAlert
          name={name}
          appearanceUrl={appearance_url}
          itemRarity={itemRarity}
          ascensionLvl={ascension_lvl}
          onClose={onClose}
        />
      );
    })
    .catch(() => {
      this.customAlertRoot.render(null);
    });
  }

  render() {
    const { tPrice, CoinsData, ePrice, eLvl } = this.state;

    return (
      <div>
        <div className="top" style={{ borderBottom: "3px solid #7289da" }}>
          <div className="left-base">
            <div className="column align-center">
              <div className="text-block" style={{ width: "430px" }}>
                <div className="text-bold text-30px">
                  Стоимость билета призыва
                </div>
                <div className="text-30px" style={{ marginTop: "5px" }}>
                  {tPrice && tPrice.ticket_price}
                </div>
              </div>
            </div>
          </div>
          <div className="right-base">
            <div className="column align-center">
              <button
                className="button-long text-30px"
                onClick={() => this.buyTicket()}
              >
                Купить билет
              </button>
              <button
                className="button-long text-30px"
                onClick={() => this.summonChar()}
              >
                Призвать персонажа
              </button>
              <div
                className="text-30px text-white"
                style={{ marginTop: "5px" }}
              >
                {CoinsData && CoinsData.curr_ticket}
              </div>
            </div>
          </div>
        </div>
        <div id="custom-alert-root"></div>
        <div className="bottom">
          <div className="left-base">
            <div className="column align-center">
              <div className="text-block" style={{ width: "430px" }}>
                <div className="text-bold text-30px">Монеты</div>
                <div className="text-30px" style={{ marginTop: "5px" }}>
                  {CoinsData && CoinsData.coins}
                </div>
              </div>
            </div>
          </div>
          <div className="right-base">
            <div className="column">
              <div className="row">
                <button
                  className="button-short text-30px"
                  onClick={() => this.upgradeEquip("lvl_weapon")}
                >
                  Оружие {eLvl && eLvl.lvl_weapon}:{" "}
                  {ePrice && ePrice.weapon_price}
                </button>
                <button
                  className="button-short text-30px"
                  onClick={() => this.upgradeEquip("lvl_boots")}
                >
                  Ботинки {eLvl && eLvl.lvl_boots}:{" "}
                  {ePrice && ePrice.boots_price}
                </button>
              </div>
              <div className="row">
                <button
                  className="button-short text-30px"
                  onClick={() => this.upgradeEquip("lvl_armor")}
                >
                  Броня {eLvl && eLvl.lvl_armor}: {ePrice && ePrice.armor_price}
                </button>
                <button
                  className="button-short text-30px"
                  onClick={() => this.upgradeEquip("lvl_gloves")}
                >
                  Перчатки {eLvl && eLvl.lvl_gloves}:{" "}
                  {ePrice && ePrice.gloves_price}
                </button>
              </div>
              <div className="row">
                <button
                  className="button-short text-30px"
                  onClick={() => this.upgradeEquip("lvl_greaves")}
                >
                  Поножи {eLvl && eLvl.lvl_greaves}:{" "}
                  {ePrice && ePrice.greaves_price}
                </button>
                <button
                  className="button-short text-30px"
                  onClick={() => this.upgradeEquip("lvl_jewelry")}
                >
                  Бижутерия {eLvl && eLvl.lvl_jewelry}:{" "}
                  {ePrice && ePrice.jewelry_price}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}
