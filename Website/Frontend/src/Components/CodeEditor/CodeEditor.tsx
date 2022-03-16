import { FC } from "react"
import "./CodeEditor.css";
import Editor from "@monaco-editor/react";

interface CodeEditorProps {
    code: string | null,
}

export const CodeEditor: FC<CodeEditorProps> = (props) => {
    return (
        <Editor
            onMount={(_, monaco) => {
                monaco.editor.setTheme("vs-dark");
            }}
            height="100%"
            defaultLanguage="javascript"
            value={props.code ?? ""}
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
    )
}