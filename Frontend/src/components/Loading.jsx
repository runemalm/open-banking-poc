import React from "react";
import Base from "./Base";

const Loading = ({ loadingMessage }) => {
  return (
    <Base
      title=""
      isLoading={true}
      loadingMessage={loadingMessage}
    />
  );
};

export default Loading;
