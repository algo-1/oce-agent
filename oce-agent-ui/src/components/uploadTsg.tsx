import React, { useState } from "react";
import { TextField, PrimaryButton, Stack } from "@fluentui/react";

const AddTsgForm: React.FC = () => {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [steps, setSteps] = useState("");

  const handleSubmit = async () => {
    const tsgModel = {
      title,
      description,
      steps: steps.split("\n"), // Split steps by newlines
    };

    const response = await fetch("http://localhost:5000/api/tsg", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(tsgModel),
    });

    if (response.ok) {
      alert("TSG added successfully!");
      setTitle("");
      setDescription("");
      setSteps("");
    } else {
      alert("Failed to add TSG. Please try again.");
    }
  };

  return (
    <Stack
      tokens={{ childrenGap: 15 }}
      style={{ maxWidth: 600, margin: "0 auto" }}
    >
      <h1>Add a New TSG</h1>
      <TextField
        label="Title"
        value={title}
        onChange={(_, newValue) => setTitle(newValue || "")}
        required
      />
      <TextField
        label="Description"
        value={description}
        onChange={(_, newValue) => setDescription(newValue || "")}
        multiline
        rows={3}
        required
      />
      <TextField
        label="Steps (one step per line)"
        value={steps}
        onChange={(_, newValue) => setSteps(newValue || "")}
        multiline
        rows={5}
        required
      />
      <PrimaryButton text="Add TSG" onClick={handleSubmit} />
    </Stack>
  );
};

export default AddTsgForm;
