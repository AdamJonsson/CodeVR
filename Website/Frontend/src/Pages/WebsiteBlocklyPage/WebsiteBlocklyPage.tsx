import { FC, useEffect, useRef, useState } from "react";
import Blockly from 'blockly';
import BlocklyJs from "blockly/javascript"; 
import "./WebsiteBlocklyPage.css";
import { CodeEditor } from "../../Components/CodeEditor/CodeEditor";

import ToolboxSettings from "./ToolboxCategories";

export const WebsiteBlocklyPage: FC = (props) => {

    const blocklyContainer = useRef<HTMLDivElement>(null);
    const [primaryWorkSpace, setPrimaryWorkSpace] = useState<Blockly.Workspace | null>(null)
    const [code, setCode] = useState<string>("");

    useEffect(() => {
        if (blocklyContainer.current == null) return;
        Blockly.inject(blocklyContainer.current, {toolbox: ToolboxSettings});
        setPrimaryWorkSpace(Blockly.getMainWorkspace());
    }, [blocklyContainer]);



    useEffect(() => {
        const onBlocklyWorkspaceChanged = () => {
            generateCodeFromCurrentWorkspace();
        }

        const generateCodeFromCurrentWorkspace = () => {
            if (blocklyContainer.current == null) return;
            if (primaryWorkSpace == null) return;
    
            const code = BlocklyJs.workspaceToCode(Blockly.getMainWorkspace());
            setCode(code);
        }

        if (primaryWorkSpace == null) return;
        primaryWorkSpace.addChangeListener(onBlocklyWorkspaceChanged)

        return () => {
            primaryWorkSpace.removeChangeListener(onBlocklyWorkspaceChanged)
        }
    }, [primaryWorkSpace]);


    return (
        <div className="blockly-generator-container">
            <div
                className="blockly-generator__block-container" 
                ref={blocklyContainer}></div>
            <div className="blockly-generator__code-container">
                <CodeEditor code={code}></CodeEditor>
            </div>
        </div>
    )
}