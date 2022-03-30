import { Alert, Button, Snackbar } from "@mui/material";
import { FC, useEffect, useState } from "react"
import { UnityBlocklyGenerator } from "../../Components/BlocklyGenerator/UnityBlocklyGenerator"
import { TaskPresenter } from "../../Components/TaskPresenter/TaskPresenter";
import { TaskStatus } from "../../Helpers/taskHelper";
import useUnityBlocklyXml, { WebSocketConnectionStatus } from "../../Hooks/useBlocklyXml";

import "./UnityBlocklyPage.css";

export const UnityBlocklyPage: FC = (props) => {
    const [blocklyXml, connectionStatus] = useUnityBlocklyXml();
    const [showSnackbar, setShowSnackbar] = useState(false);
    const [lastConnectionStatus, setLastConnectionStatus] = useState(WebSocketConnectionStatus.Unknown)
    
    useEffect(() => {
        if (lastConnectionStatus === connectionStatus) return;
        setLastConnectionStatus(lastConnectionStatus);
        setShowSnackbar(true);
    }, [connectionStatus, lastConnectionStatus]);

    const handleClose = () => {
        setShowSnackbar(false);
    }

    const renderAlertMessage = () => {
        if (connectionStatus === WebSocketConnectionStatus.Success) {
            return (
                <Alert onClose={handleClose} severity="success" sx={{ width: '100%' }}>
                    Successfully connected to websocket
                </Alert> 
            )
        }
        if (connectionStatus === WebSocketConnectionStatus.LostConnection) {
            return (
                <Alert action={reloadPageAction()} onClose={handleClose} severity="warning" sx={{ width: '100%' }}>
                    Lost connection to websocket
                </Alert> 
            )
        }
        if (connectionStatus === WebSocketConnectionStatus.Error) {
            return (
                <Alert action={reloadPageAction()} onClose={handleClose} severity="error" sx={{ width: '100%' }}>
                    Could not connect to websocket
                </Alert> 
            )
        }
    }
    
    const reloadPageAction = () => {
        return (
            <Button onClick={reloadPage} color="inherit" size="small">
                RELOAD PAGE
            </Button>
        )
    }

    const reloadPage = () => {
        window.location.reload();
    }

    return (
        <div className="blockly-page-container">
            <Snackbar 
                open={showSnackbar} 
                autoHideDuration={connectionStatus === WebSocketConnectionStatus.Success ? 3000 : null} 
                onClose={handleClose}>
                {renderAlertMessage()}
            </Snackbar>
            <UnityBlocklyGenerator blocklyXmlContent={blocklyXml}></UnityBlocklyGenerator>
        </div>
    )
}