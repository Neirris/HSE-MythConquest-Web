import React, { Component } from "react";
import "../../styles/pageStyles/Shop.css";

export class Shop extends Component {
  static displayName = Shop.name;

  state = {
    tPrice: null,
    CoinsData: null,
    ePrice: null,
    eLvl: null
  };

  async componentDidMount() {
        await this.getTicketPrice();
        await this.getCoinsData();
        await this.getEquipmentPrice();
        await this.getEquipmentLvl();
  }

  async getTicketPrice() {
    const response = await fetch('getshopticketprice');
    const data = await response.json();
    this.setState({ tPrice: data });
  }

  async getCoinsData() {
    const response = await fetch('getshopcoins');
    const data = await response.json();
    this.setState({ CoinsData: data });
  }

  async getEquipmentPrice() {
    const response = await fetch('getequipmentprice');
    const data = await response.json();
    this.setState({ ePrice: data });
  }

  async getEquipmentLvl() {
    const response = await fetch('getequipmentlvl');
    const data = await response.json();
    this.setState({ eLvl: data });
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
                {tPrice && (tPrice.ticket_price)}
                </div>
              </div>
            </div>
          </div>
          <div className="right-base">
            <div className="column align-center">
              <button className="button-long text-30px">Купить билет</button>
              <button className="button-long text-30px">
                Призвать персонажа
              </button>
              <div
                className="text-30px text-white"
                style={{ marginTop: "5px" }}
              >
                {CoinsData && (CoinsData.curr_ticket)}
              </div>
            </div>
          </div>
        </div>
        <div className="bottom">
          <div className="left-base">
            <div className="column align-center">
              <div className="text-block" style={{ width: "430px" }}>
                <div className="text-bold text-30px">Монеты</div>
                <div className="text-30px" style={{ marginTop: "5px" }}>{CoinsData && (CoinsData.coins)}</div>
              </div>
            </div>
          </div>
          <div className="right-base">
            <div className="column">
            <div className="row">
              <button className="button-short text-30px">Оружие {eLvl && (eLvl.lvl_weapon)}: {ePrice && (ePrice.weapon_price)}</button>
              <button className="button-short text-30px">Ботинки {eLvl && (eLvl.lvl_boots)}: {ePrice && (ePrice.boots_price)}</button>
            </div>
            <div className="row">
              <button className="button-short text-30px">Броня {eLvl && (eLvl.lvl_armor)}: {ePrice && (ePrice.armor_price)}</button>
              <button className="button-short text-30px">Перчатки {eLvl && (eLvl.lvl_gloves)}: {ePrice && (ePrice.gloves_price)}</button>
            </div>
            <div className="row">
              <button className="button-short text-30px">Поножи {eLvl && (eLvl.lvl_greaves)}: {ePrice && (ePrice.greaves_price)}</button>
              <button className="button-short text-30px">Бижутерия {eLvl && (eLvl.lvl_jewelry)}: {ePrice && (ePrice.jewelry_price)}</button>
            </div>
            </div>      
          </div>
        </div>
      </div>
    );
  }
}
