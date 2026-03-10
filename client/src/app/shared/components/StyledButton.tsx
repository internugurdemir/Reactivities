import { Button, type ButtonProps, styled } from "@mui/material";
import { type LinkProps } from "react-router";
//https://mui.com/material-ui/customization/creating-themed-components/
type StyledButtonProps = ButtonProps & Partial<LinkProps>;

const StyledButton = styled(Button)<StyledButtonProps>(({ theme }) => ({
    '&.Mui-disabled': {
        backgroundColor: theme.palette.grey[600],
        color: theme.palette.text.disabled,
    },
}));

export default StyledButton;