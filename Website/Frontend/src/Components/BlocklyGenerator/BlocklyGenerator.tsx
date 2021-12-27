import { FC, useEffect, useRef, useState } from "react"
import Blockly from 'blockly';
import BlocklyJs from "blockly/javascript"; 

import "./BlocklyGenerator.css";
import Editor from "@monaco-editor/react";

interface BlocklyGeneratorProps {
    blocklyXmlContent: string | null,
}

export const BlocklyGenerator: FC<BlocklyGeneratorProps> = (props) => {
    const blocklyContainer = useRef<HTMLDivElement>(null);
    const [primaryWorkSpace, setPrimaryWorkSpace] = useState<Blockly.Workspace | null>(null)
    const [code, setCode] = useState<string>("");

    useEffect(() => {
        if (blocklyContainer.current == null) return;
        Blockly.inject(blocklyContainer.current, {});
        setPrimaryWorkSpace(Blockly.getMainWorkspace());
    }, [blocklyContainer]);

    useEffect(() => {
        if (blocklyContainer.current == null) return;
        if (props.blocklyXmlContent == null) return;
        if (primaryWorkSpace == null) return;

        primaryWorkSpace.clear();
        Blockly.Xml.domToWorkspace(Blockly.Xml.textToDom(props.blocklyXmlContent), primaryWorkSpace!);

        const code = BlocklyJs.workspaceToCode(Blockly.getMainWorkspace());
        setCode(code);
    }, [blocklyContainer, props.blocklyXmlContent, primaryWorkSpace]);

    return (
        <div className="blockly-generator-container">
            <div className="blockly-generator__block-container" ref={blocklyContainer}></div>
            <div className="blockly-generator__code-container">
                <Editor
                    onMount={(_, monaco) => {
                        monaco.editor.setTheme("vs-dark");
                    }}
                    height="100%"
                    defaultLanguage="javascript"
                    value={code}
                    options = {{
                        theme: "vs-dark",
                        scrollBeyondLastLine: false,
                        padding: {
                            top: 25,
                            bottom: 25,
                        },
                        readOnly: true,
                    }}
                />
            </div>
        </div>
    )
}