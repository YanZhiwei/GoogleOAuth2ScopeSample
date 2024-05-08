import os.path
import os
from google.auth.transport.requests import Request
from google.oauth2.credentials import Credentials
from google_auth_oauthlib.flow import InstalledAppFlow
from googleapiclient.discovery import build
from googleapiclient.errors import HttpError

# If modifying these scopes, delete the file token.json.
SCOPES = ["https://www.googleapis.com/auth/spreadsheets"]

# The ID and range of a sample spreadsheet.
SAMPLE_SPREADSHEET_ID = "1Jpfb9sOoWlPR_DhW4mSPHd7WI3jgh0GV7S3172IyryY"
SAMPLE_RANGE_NAME = "Class Data!A2:E"


def main():
    """Shows basic usage of the Sheets API.
    Prints values from a sample spreadsheet.
    https://developers.google.com/sheets/api/guides/create?hl=zh-cn#python
    """
    creds = Credentials(
        refresh_token="{refresh_token}",
        token="{token}",
        token_uri="https://oauth2.googleapis.com/token",
        client_id="{client_id}",
        client_secret="{client_secret}",
    )
    # If there are no (valid) credentials available, let the user log in.
    if not creds or not creds.valid:
        if creds and creds.expired and creds.refresh_token:
            creds.refresh(Request())
        else:
            flow = InstalledAppFlow.from_client_secrets_file(
                os.path.join(os.getcwd(), "Web_OAuth", "credentials.json"), SCOPES
            )
            creds = flow.run_local_server(port=0)

    try:
        sheetid = create(creds, "mysheet2")
        update_values(
            creds,
            sheetid,
            "A1:C2",
            "USER_ENTERED",
            [["A", "B"], ["C", "D"]],
        )
    except HttpError as err:
        print(err)


def update_values(creds, spreadsheet_id, range_name, value_input_option, _values):
    """
    Creates the batch_update the user has access to.
    Load pre-authorized user credentials from the environment.
    TODO(developer) - See https://developers.google.com/identity
    for guides on implementing OAuth2 for the application.
    """
    # pylint: disable=maybe-no-member
    try:
        service = build("sheets", "v4", credentials=creds)
        values = [["aa", "bb"], ["cc", "dd"]]
        body = {"values": values}
        result = (
            service.spreadsheets()
            .values()
            .update(
                spreadsheetId=spreadsheet_id,
                range=range_name,
                valueInputOption=value_input_option,
                body=body,
            )
            .execute()
        )
        print(f"{result.get('updatedCells')} cells updated.")
        return result
    except HttpError as error:
        print(f"An error occurred: {error}")
        return error


def create(creds, title):
    """
    Creates the Sheet the user has access to.
    Load pre-authorized user credentials from the environment.
    TODO(developer) - See https://developers.google.com/identity
    for guides on implementing OAuth2 for the application.
    """
    # pylint: disable=maybe-no-member
    try:
        service = build("sheets", "v4", credentials=creds)
        spreadsheet = {"properties": {"title": title}}
        spreadsheet = (
            service.spreadsheets()
            .create(body=spreadsheet, fields="spreadsheetId")
            .execute()
        )
        print(f"Spreadsheet ID: {(spreadsheet.get('spreadsheetId'))}")
        return spreadsheet.get("spreadsheetId")
    except HttpError as error:
        print(f"An error occurred: {error}")
        return error


if __name__ == "__main__":
    main()
