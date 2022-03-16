import { Button } from "@mui/material";
import { FC } from "react"
import { Link } from "react-router-dom";
import "./HomePage.css";

export const HomePage: FC = (props) => {
    return (
        <div className="home-page__container">
            <div className="home-page__button-container">
                <div className="home-page__button">
                    <Link to="/Unity">
                        <Button variant="contained"><b>Unity</b></Button>
                    </Link>
                </div>
                <br />
                <div className="home-page__button">
                    <Link to="/Unity/Debug">
                        <Button variant="contained"><b>Unity (Debug)</b></Button>
                    </Link>
                </div>
                <br />
                <div className="home-page__button">
                    <Link to="/Website">
                        <Button variant="contained"><b>Website</b></Button>
                    </Link>
                </div>
            </div>
        </div>
    )
}