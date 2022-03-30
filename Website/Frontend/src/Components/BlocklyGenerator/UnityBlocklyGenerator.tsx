import { FC, useEffect, useRef, useState } from "react"
import Blockly from 'blockly';
import BlocklyJs from "blockly/javascript"; 

import "./UnityBlocklyGenerator.css";
import { CodeEditor } from "../CodeEditor/CodeEditor";
import { useLocation } from "react-router-dom";
import getCurrentTaskStatus, { TaskStatus } from "../../Helpers/taskHelper";
import { TaskPresenter } from "../TaskPresenter/TaskPresenter";

interface UnityBlocklyGeneratorProps {
    blocklyXmlContent: string | null,
}

export const UnityBlocklyGenerator: FC<UnityBlocklyGeneratorProps> = (props) => {
    const blocklyContainer = useRef<HTMLDivElement>(null);
    const [primaryWorkSpace, setPrimaryWorkSpace] = useState<Blockly.Workspace | null>(null)
    const [code, setCode] = useState<string>("");
    const [activeTask, setActiveTask] = useState<TaskStatus | null>(null);
    const [loadingNextTask, setLoadingNextTask] = useState<boolean>(true);
    const location = useLocation();
    const debugMode = location.pathname === "/Unity/Debug";

    useEffect(() => {
        if (primaryWorkSpace == null) return;
        async function fetchTaskStatus() {
            setLoadingNextTask(true);
            var taskStatus = await getCurrentTaskStatus();
            setActiveTask(taskStatus);
            setLoadingNextTask(false);
        }
        fetchTaskStatus();
    }, [primaryWorkSpace]);

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
            <div
                style={{display: debugMode ? "block" : "none"}}
                className="blockly-generator__block-container" 
                ref={blocklyContainer}></div>
            <div className="blockly-generator__code-container">
                <CodeEditor code={code}></CodeEditor>
            </div>
            {
                activeTask != null 
                ?
                    <TaskPresenter 
                        code={code} 
                        task={activeTask.task}
                        isLoadingNextTask={loadingNextTask}
                        isLastTask={activeTask.isLastTask}
                        onNextTaskButtonClicked={() => {}}
                        ></TaskPresenter>
                :
                    <></>
            }
        </div>
    )
}