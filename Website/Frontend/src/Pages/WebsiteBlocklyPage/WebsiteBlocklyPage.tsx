import { FC, useCallback, useEffect, useRef, useState } from "react";
import Blockly from 'blockly';
import BlocklyJs from "blockly/javascript"; 
import "./WebsiteBlocklyPage.css";
import { CodeEditor } from "../../Components/CodeEditor/CodeEditor";

import ToolboxSettings from "./ToolboxCategories";
import { TaskPresenter } from "../../Components/TaskPresenter/TaskPresenter";

import getCurrentTaskStatus, { TaskStatus, moveToNextTask } from "../../Helpers/taskHelper";
import taskStartBlockWebsite from "../../Helpers/taskStartBlocksWebsite";

export const WebsiteBlocklyPage: FC = (props) => {

    const blocklyContainer = useRef<HTMLDivElement>(null);
    const [primaryWorkSpace, setPrimaryWorkSpace] = useState<Blockly.Workspace | null>(null)
    const [code, setCode] = useState<string>("");
    const [activeTask, setActiveTask] = useState<TaskStatus | null>(null);
    const [loadingNextTask, setLoadingNextTask] = useState<boolean>(true);


    const changeActiveTask = useCallback((taskStatus: TaskStatus) => {
        if (primaryWorkSpace == null) return;
        setActiveTask(taskStatus);
        var startBlocks = taskStartBlockWebsite[taskStatus.task.id];

        primaryWorkSpace!.clear();
        Blockly.Xml.domToWorkspace(Blockly.Xml.textToDom(startBlocks), primaryWorkSpace!);
    }, [primaryWorkSpace]);


    useEffect(() => {
        if (primaryWorkSpace == null) return;
        async function fetchTaskStatus() {
            setLoadingNextTask(true);
            var taskStatus = await getCurrentTaskStatus();
            changeActiveTask(taskStatus);
            setLoadingNextTask(false);
        }
        fetchTaskStatus();
    }, [primaryWorkSpace, changeActiveTask]);

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

    const loadNextTask = async () => {
        setLoadingNextTask(true);
        await moveToNextTask()
        var taskStatus = await getCurrentTaskStatus();
        changeActiveTask(taskStatus);
        setLoadingNextTask(false);
    }

    return (
        <div className="website-blockly-page__container">
            <div
                className="website-blockly-page__blockly" 
                ref={blocklyContainer}></div>
            <div className="website-blockly-page__right-tools">
                <div className="website-blockly-page__task">
                    {
                        activeTask != null 
                        ?
                            <TaskPresenter 
                                code={code} 
                                task={activeTask.task}
                                isLoadingNextTask={loadingNextTask}
                                isLastTask={activeTask.isLastTask}
                                onNextTaskButtonClicked={loadNextTask}
                                ></TaskPresenter>
                        :
                            <></>
                    }
                </div>
                <div className="website-blockly-page__code">
                    <CodeEditor code={code}></CodeEditor>
                </div>
            </div>
        </div>
    )
}