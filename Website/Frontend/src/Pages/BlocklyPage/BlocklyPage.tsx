import { FC } from "react"
import { BlocklyGenerator } from "../../Components/BlocklyGenerator/BlocklyGenerator"
import useBlocklyXml from "../../Hooks/useBlocklyXml";

import "./BlocklyPage.css";

export const BlocklyPage: FC = (props) => {
    const blocklyXml = useBlocklyXml();

    return (
        <div className="blockly-page-container">
            <BlocklyGenerator blocklyXmlContent={blocklyXml}></BlocklyGenerator>
        </div>
    )
}