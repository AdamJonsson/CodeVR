import { FC, useEffect, useRef, useState } from "react";
import Task from "../../Types/Task";
import testBlocklyCode from "../../Helpers/testBlocklyCode";

import "./TaskPresenter.css";
import { Button } from "@mui/material";
import { updateCurrentTaskStatus } from "../../Helpers/taskHelper";

interface TaskPresenterProps {
    code: string,
    task: Task,
    isLastTask: boolean,
    isLoadingNextTask: boolean,
    onNextTaskButtonClicked: () => void,
}

export const TaskPresenter: FC<TaskPresenterProps> = (props) => {
    
    const [taskState, setTaskState] = useState<{taskCompleted: boolean, inputs: string, expectedOutput: string, output: string}>({
        taskCompleted: false,
        inputs: "",
        expectedOutput: "",
        output: ""
    });

    useEffect(() => {
        testBlocklyCode(props.code, props.task).then(data => {
            var [taskCompleted, inputs, expectedOutput, output] = data;
            updateCurrentTaskStatus(
                taskCompleted, 
                taskCompleted ? null : {
                    inputs: inputs,
                    output: expectedOutput,
                },
                output
            );
    
            setTaskState({
                taskCompleted: taskCompleted,
                inputs: inputs,
                expectedOutput: expectedOutput,
                output: output
            })
        });

    }, [props.code, props.task]);

    const onNextTaskButtonClicked = () => {
        if (props.isLoadingNextTask) return;
        props.onNextTaskButtonClicked();
    }

    return (
        <div className="task-presenter__container">
            
            <div className="task-presenter__description">
                <h3>{props.task.title}</h3>
                <p>
                    { props.task.description }
                </p>
            </div>

            <div className="task-presenter__task-state">
                <h3 className="task-presenter__completed-title">
                    {
                        taskState.taskCompleted === true ?
                            <div style={{color: "#00aa10"}}>
                                Success
                            </div>
                        :
                            <div style={{color: "#aa0000"}}>
                                Tests failed
                            </div>
                    }
                </h3>
            </div>
            
            {
                taskState.taskCompleted === true
                ?
                    <>
                        {
                            props.isLastTask
                            ?
                                <>
                                    All tasks have been solved!
                                </>
                            :
                                <>
                                    All tests passed! You can now move on to the next task.
                                    <br></br><br></br>
                                    <Button onClick={onNextTaskButtonClicked} variant="contained">
                                        <b>
                                            {
                                                props.isLoadingNextTask ? "Loading..." : "Load next task"
                                            }
                                        </b>
                                    </Button>
                                </>
                        }
                    </>
                :
                    <>
                        When the inputs is:
                        <div className="task-presenter__values">
                            {taskState.inputs}
                        </div>

                        The output should be:
                        <div className="task-presenter__values">
                            {taskState.expectedOutput}
                        </div>
                        
                        But the output is currently:
                        <div className="task-presenter__values">
                            {
                                taskState.output
                                ?
                                    taskState.output
                                : 
                                    <i>EMPTY</i>
                            }
                        </div>
                    </>
            }

        </div>
    )
}