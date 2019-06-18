![](https://github.com/Microsoft/MCW-Template-Cloud-Workshop/raw/master/Media/ms-cloud-workshop.png 'Microsoft Cloud Workshops')

<div class="MCWHeader1">
Managed open source databases on Azure
</div>

<div class="MCWHeader2">
Hands-on lab step-by-step
</div>

<div class="MCWHeader3">
June 2019
</div>

Information in this document, including URL and other Internet Web site references, is subject to change without notice. Unless otherwise noted, the example companies, organizations, products, domain names, e-mail addresses, logos, people, places, and events depicted herein are fictitious, and no association with any real company, organization, product, domain name, e-mail address, logo, person, place or event is intended or should be inferred. Complying with all applicable copyright laws is the responsibility of the user. Without limiting the rights under copyright, no part of this document may be reproduced, stored in or introduced into a retrieval system, or transmitted in any form or by any means (electronic, mechanical, photocopying, recording, or otherwise), or for any purpose, without the express written permission of Microsoft Corporation.

Microsoft may have patents, patent applications, trademarks, copyrights, or other intellectual property rights covering subject matter in this document. Except as expressly provided in any written license agreement from Microsoft, the furnishing of this document does not give you any license to these patents, trademarks, copyrights, or other intellectual property.

The names of manufacturers, products, or URLs are provided for informational purposes only and Microsoft makes no representations and warranties, either expressed, implied, or statutory, regarding these manufacturers or the use of the products with any Microsoft technologies. The inclusion of a manufacturer or product does not imply endorsement of Microsoft of the manufacturer or product. Links may be provided to third party sites. Such sites are not under the control of Microsoft and Microsoft is not responsible for the contents of any linked site or any link contained in a linked site, or any changes or updates to such sites. Microsoft is not responsible for webcasting or any other form of transmission received from any linked site. Microsoft is providing these links to you only as a convenience, and the inclusion of any link does not imply endorsement of Microsoft of the site or the products contained therein.

© 2019 Microsoft Corporation. All rights reserved.

Microsoft and the trademarks listed at <https://www.microsoft.com/en-us/legal/intellectualproperty/Trademarks/Usage/General.aspx> are trademarks of the Microsoft group of companies. All other trademarks are property of their respective owners.

**Contents**

<!-- TOC -->

- [Managed open source databases on Azure hands-on lab step-by-step](#Managed-open-source-databases-on-Azure-hands-on-lab-step-by-step)
  - [Abstract and learning objectives](#Abstract-and-learning-objectives)
  - [Overview](#Overview)
  - [Solution architecture](#Solution-architecture)
  - [Requirements](#Requirements)
  - [Before the hands-on lab](#Before-the-hands-on-lab)
  - [Exercise 1: Connect to and set up your database](#Exercise-1-Connect-to-and-set-up-your-database)
    - [Task 1: Connect to the PostgreSQL database](#Task-1-Connect-to-the-PostgreSQL-database)
    - [Task 2: Create a table to store clickstream data](#Task-2-Create-a-table-to-store-clickstream-data)
    - [Task 3: Shard tables across nodes](#Task-3-Shard-tables-across-nodes)
  - [Exercise 2: Add secrets to Key Vault and configure Azure Databricks](#Exercise-2-Add-secrets-to-Key-Vault-and-configure-Azure-Databricks)
    - [Task 1: Obtain and store ADLS Gen2 secrets in Azure Key Vault](#Task-1-Obtain-and-store-ADLS-Gen2-secrets-in-Azure-Key-Vault)
    - [Task 2: Obtain and store Event Hubs (Kafka) secrets in Azure Key Vault](#Task-2-Obtain-and-store-Event-Hubs-Kafka-secrets-in-Azure-Key-Vault)
    - [Task 3: Create a service principal for OAuth access to the ADLS Gen2 filesystem](#Task-3-Create-a-service-principal-for-OAuth-access-to-the-ADLS-Gen2-filesystem)
    - [Task 4: Add the service principal credentials and Tenant Id to Azure Key Vault](#Task-4-Add-the-service-principal-credentials-and-Tenant-Id-to-Azure-Key-Vault)
    - [Task 5: Create an Azure Databricks cluster](#Task-5-Create-an-Azure-Databricks-cluster)
    - [Task 6: Load lab notebooks into Azure Databricks](#Task-6-Load-lab-notebooks-into-Azure-Databricks)
    - [Task 7: Configure Azure Databricks Key Vault-backed secrets](#Task-7-Configure-Azure-Databricks-Key-Vault-backed-secrets)
  - [Exercise 3: Send clickstream data to Kafka and process it in real time](#Exercise-3-Send-clickstream-data-to-Kafka-and-process-it-in-real-time)
    - [Task 1: Configure the KafkaProducer application](#Task-1-Configure-the-KafkaProducer-application)
    - [Task 2: Open notebook and process the streaming data](#Task-2-Open-notebook-and-process-the-streaming-data)
  - [After the hands-on lab](#After-the-hands-on-lab)
    - [Task 1: Task name](#Task-1-Task-name)
    - [Task 2: Task name](#Task-2-Task-name)

<!-- /TOC -->

# Managed open source databases on Azure hands-on lab step-by-step

## Abstract and learning objectives

\[Insert what is trying to be solved for by using this workshop. . . \]

## Overview

\[insert your custom workshop content here . . . \]

## Solution architecture

\[Insert your end-solution architecture here. . .\]

## Requirements

1.  Number and insert your custom workshop content here . . .

## Before the hands-on lab

Refer to the Before the hands-on lab setup guide manual before continuing to the lab exercises.

## Exercise 1: Connect to and set up your database

Duration: 15 minutes

In this exercise, you will obtain your PostgreSQL connection string and use the pgAdmin tool to connect and create your schema for this lab.

### Task 1: Connect to the PostgreSQL database

1. Open the [Azure portal](https://portal.azure.com) and navigate to the resource group you created (`hands-on-lab-SUFFIX` where SUFFIX is your unique identifier).

2. Find your PostgreSQL server group and select it. (The server group name will not have a suffix. Items with names ending in, for example, "-c", "-w0", or "-w1" are not the server group.)

   ![The PostgreSQL server group is highlighted in the resource group.](media/resource-group-pg-server-group.png 'Resource group')

3. Select **Connection strings** in the left-hand menu. Copy the string marked **JDBC**.

   ![The Connection strings item is selected and the JDBC connection string is highlighted.](media/postgres-jdbc-connection-string.png 'Connection strings')

4. Replace "{your_password}" with the administrative password you chose earlier (such as `Abc!1234567890`). The system doesn't store your plaintext password and so can't display it for you in the connection string. **Save the connection string** to Notebook or similar text editor for later.

5. Launch pgAdmin. Select **Add New Server** on the home page.

   ![The pgAdmin home page is displayed with Add New Server highlighted.](media/pgadmin-home.png 'pgAdmin')

6. In the **General** tab of the Create Server dialog, enter **Lab** into the Name field.

   ![The Name field is filled out in the General tab.](media/pgadmin-create-server-general.png 'Create Server - General tab')

7. Select the **Connection** tab. Enter the following into the fields within the Connection tab:

   - **Host name/address**: paste the host value from the connection string you copied earlier (the string of text between `jdbc:postgresql://` and `:5432`. For example: `<your-server-name>.postgres.database.azure.com`)
   - **Port**: 5432
   - **Maintenance database**: citus
   - **Username**: citus
   - **Password**: the administrative password you chose earlier (such as `Abc!1234567890`)
   - **Save password?**: check the box

   ![The previously described fields are filled in within the Connection tab.](media/pgadmin-create-server-connection.png 'Create Server - Connection tab')

8. Click the **Save** button.

9. Expand the newly added **Lab** server under the Servers tree on the pgAdmin home page. You should be able to expand the citus database.

   ![The pgAdmin home page is displayed and the Lab server is expanded.](media/pgadmin-home-connected.png 'pgAdmin home')

### Task 2: Create a table to store clickstream data

In this task, you will create the `events` raw table to capture every clickstream event. This table is partitioned by `event_time` since we are using it to store time series data. The script you execute to create the schema creates a partition every 5 minutes, using [pg_partman](https://www.citusdata.com/blog/2018/01/24/citus-and-pg-partman-creating-a-scalable-time-series-database-on-PostgreSQL/).

1. With the **Lab** server expanded under the Servers tree in pgAdmin, expand Databases then select **citus**. When the citus database is highlighted, select the **Query Tool** button above.

   ![The citus database is selected in pgAdmin, and the Query Tool is highlighted.](media/pgadmin-query-tool-button.png 'Query Tool')

2. Paste the following query into the Query Editor:

   ```sql
   CREATE TABLE events(
       event_id serial,
       event_time timestamptz default now(),
       customer_id bigint,
       event_type text,
       country text,
       browser text,
       device_id bigint,
       session_id bigint
   )
   PARTITION BY RANGE (event_time);

   --Create 5-minutes partitions
   SELECT partman.create_parent('public.events', 'event_time', 'native', '5 minutes');
   UPDATE partman.part_config SET infinite_time_partitions = true;

   SELECT create_distributed_table('events','customer_id');
   ```

3. Press F5 to execute the query, or select the **Execute** button on the toolbar above.

   ![The execute button is highlighted in the Query Editor.](media/pgadmin-query-editor-execute.png 'Query Editor')

4. After executing the query, verify that the new `events` table was created under the **citus** database by expanding **Schemas** -> **public** -> **Tables** in the navigation tree on the left. You may have to refresh the Schemas list by right-clicking, then selecting Refresh.

   ![The new events table is displayed in the navigation tree on the left.](media/pgadmin-events-table.png 'Events table')

### Task 3: Shard tables across nodes

In this task, you will create two rollup tables for storing aggregated data pulled from the raw events table. Later, you will create rollup functions and schedule them to run periodically.

The two tables you will create are:

- **rollup_events_5mins**: stores aggregated data in 5-minute intervals.
- **rollup_events_1hr**: stores aggregated data every 1 hour.

You will notice in the script below, as well as in the script above, that we are sharding each of the tables on `customer_id` column. The sharding logic is handled for you by the Hyperscale server group (enabled by Citus), allowing you to horizontally scale your database across multiple managed Postgres servers.

1. With the **Lab** server expanded under the Servers tree in pgAdmin, expand Databases then select **citus**. When the citus database is highlighted, select the **Query Tool** button above.

   ![The citus database is selected in pgAdmin, and the Query Tool is highlighted.](media/pgadmin-query-tool-button.png 'Query Tool')

2. Paste the following query into the Query Editor:

   ```sql
   CREATE TABLE rollup_events_5min (
        customer_id bigint,
        event_type text,
        country text,
        browser text,
        minute timestamptz,
        event_count bigint,
        device_distinct_count hll,
        session_distinct_count hll,
        top_devices_1000 jsonb
    );
    CREATE UNIQUE INDEX rollup_events_5min_unique_idx ON rollup_events_5min(customer_id,event_type,country,browser,minute);
    SELECT create_distributed_table('rollup_events_5min','customer_id');

    CREATE TABLE rollup_events_1hr (
        customer_id bigint,
        event_type text,
        country text,
        browser text,
        hour timestamptz,
        event_count bigint,
        device_distinct_count hll,
        session_distinct_count hll,
        top_devices_1000 jsonb
    );
    CREATE UNIQUE INDEX rollup_events_1hr_unique_idx ON rollup_events_1hr(customer_id,event_type,country,browser,hour);
    SELECT create_distributed_table('rollup_events_1hr','customer_id');
   ```

3. Press F5 to execute the query, or select the **Execute** button on the toolbar above.

4. After executing the query, verify that the new `rollup_events_1hr` and `rollup_events_5min` tables were created under the **citus** database by expanding **Schemas** -> **public** -> **Tables** in the navigation tree on the left. You may have to refresh the Schemas list by right-clicking, then selecting Refresh.

   ![The new rollup tables are displayed in the navigation tree on the left.](media/pgadmin-rollup-tables.png 'Rollup tables')

## Exercise 2: Add secrets to Key Vault and configure Azure Databricks

Duration: 30 minutes

In this exercise, you will add secrets to Key Vault to securely store secrets, such as your PostgreSQL database, Event Hubs (Kafka), and Azure Data Lake Storage Gen2 credentials. You will then create a new Databricks cluster configure your Azure Databricks workspace to use a Key Vault-backed secret store to access those secrets.

### Task 1: Obtain and store ADLS Gen2 secrets in Azure Key Vault

1. Open the [Azure portal](https://portal.azure.com) and navigate to the resource group you created (`hands-on-lab-SUFFIX` where SUFFIX is your unique identifier).

2. Find your storage account (`wwiadlsSUFFIX`) and select it.

   ![The storage account is highlighted within the resource group.](media/resource-group-storage-account.png 'Storage account')

3. Select **Access keys** under Settings on the left-hand menu. You are going to copy the **Storage account name** and **Key** values and add them as secrets in your Key Vault account.

   ![The storage account Access keys blade is displayed, and the Storage account name and Key are highlighted.](media/storage-account-keys.png 'Access keys')

4. Open a new browser tab or window and navigate to your Azure Key Vault account in the Azure portal, then select **Secrets** under Settings on the left-hand menu. On the Secrets blade, select **+ Generate/Import** on the top toolbar.

   ![In Key Vault, Secrets is selected in the left-hand menu, and the Generate/Import button is highlighted.](media/key-vault-generate-secret.png 'Secrets')

5. On the Create a secret blade, enter the following:

   - **Upload options**: Select Manual.
   - **Name**: Enter "ADLS-Gen2-Account-Name".
   - **Value**: Paste the Storage account name value you copied in an earlier step.

   ![The Create a secret form is displayed with the previously defined field values.](media/key-vault-add-account-name.png 'Create a secret')

6. Select **Create**.

7. Select **+ Generate/Import** again on the top toolbar to create another secret.

8. On the Create a secret blade, enter the following:

   - **Upload options**: Select Manual.
   - **Name**: Enter "ADLS-Gen2-Account-Key".
   - **Value**: Paste the Storage account Key value you copied in an earlier step.

   ![The Create a secret form is displayed with the previously defined field values.](media/key-vault-add-account-key.png 'Create a secret')

9. Select **Create**.

10. Select **+ Generate/Import** again on the top toolbar to create another secret.

11. On the Create a secret blade, enter the following:

    - **Upload options**: Select Manual.
    - **Name**: Enter "Database-Connection-String".
    - **Value**: Paste the PostgreSQL JDBC connection string you copied in an earlier step. Make sure it contains your password.

    ![The Create a secret form is displayed with the previously defined field values.](media/key-vault-add-connection-string.png 'Create a secret')

12. Select **Create**.

### Task 2: Obtain and store Event Hubs (Kafka) secrets in Azure Key Vault

In this task, you will obtain the Event Hubs connection string and store it as a secret in Azure Key Vault, along with the fully qualified domain name. This information will be used by Apache Spark within Azure Databricks to consume and process the streaming data using the Kafka protocol.

1. Open the [Azure portal](https://portal.azure.com) and navigate to the resource group you created (`hands-on-lab-SUFFIX` where SUFFIX is your unique identifier).

2. Find your Event Hubs namespace (`wwi-namespace-SUFFIX`) and select it.

   ![The Event Hubs Namespace is highlighted within the resource group.](media/resource-group-event-hubs.png 'Event Hubs Namespace')

3. Select **Shared access policies** under Settings in the left-hand menu, then select the **RootManageSharedAccessKey** policy.

   ![The Event Hubs shared access policies blade is displayed.](media/event-hubs-shared-access-policies.png 'Shared access policies')

4. Copy the **Connection string-primary key** value and save it to Notepad or similar text editor.

   ![The SAS Policy is displayed and the copy button for the connection string is highlighted.](media/event-hubs-shared-access-policy.png 'SAS Policy')

5. Open a new browser tab or window and navigate to your Azure Key Vault account in the Azure portal, then select **Secrets** under Settings on the left-hand menu. On the Secrets blade, select **+ Generate/Import** on the top toolbar.

   ![In Key Vault, Secrets is selected in the left-hand menu, and the Generate/Import button is highlighted.](media/key-vault-generate-secret.png 'Secrets')

6. On the Create a secret blade, enter the following:

   - **Upload options**: Select Manual.
   - **Name**: Enter "Kafka-Connection-String".
   - **Value**: Paste the Event Hubs connection string you copied in an earlier step.

   ![The Create a secret form is displayed with the previously defined field values.](media/key-vault-add-kafka-connection-string.png 'Create a secret')

7. Select **Create**.

8. Select **+ Generate/Import** again on the top toolbar to create another secret.

9. On the Create a secret blade, enter the following:

   - **Upload options**: Select Manual.
   - **Name**: Enter "Kafka-Server".
   - **Value**: Paste the fully qualified domain name (FQDN) that points to your Event Hubs namespace from the connection string (eg. `wwi-namespace-SUFFIX.servicebus.windows.net`). The FQDN can be found within your connection string as follows:

   `Endpoint=sb://` **wwi-namespace-SUFFIX.servicebus.windows.net** `/;SharedAccessKeyName=XXXXXX;SharedAccessKey=XXXXXX`

   ![The Create a secret form is displayed with the previously defined field values.](media/key-vault-add-kafka-server.png 'Create a secret')

10. Select **Create**.

### Task 3: Create a service principal for OAuth access to the ADLS Gen2 filesystem

As an added layer of security when accessing an ADLS Gen2 filesystem using Databricks you can use OAuth 2.0 for authentication. In this task, you will use the Azure CLI to create an identity in Azure Active Directory (Azure AD) known as a service principal to facilitate the use of OAuth authentication.

> **IMPORTANT**: You must have permissions within your Azure subscription to create an App registration and service principal within Azure Active Directory to complete this task.

1. In the [Azure portal](https://portal.azure.com), select the **Cloud Shell** icon in the top toolbar.

   ![The Azure Cloud Shell icon is highlighted in the Azure portal's top menu.](media/cloud-shell-icon.png 'Azure Cloud Shell')

2. Ensure **Bash** is selected in the Cloud Shell pane.

   ![The Azure Cloud Shell is displayed with its default prompt.](media/cloud-shell-prompt.png 'Azure Cloud Shell')

3. If you have multiple subscriptions, choose the appropriate subscription in which the resource should be billed. List all your subscriptions by entering the following into the shell:

   ```bash
   az account list
   ```

4. Select the specific subscription ID under your account using `az account set` command. Copy the `id` value from the output of the previous command for the subscription you wish to use into the `subscription id` placeholder:

   ```bash
   az account set --subscription <subscription id>
   ```

5. Next, you will issue a command to create a service principal named **wwi-oss-sp** and assign it to the _Storage Blob Data Contributor_ role on your **ADLS Gen2 Storage account**. The command will be in the following format:

   ```bash
   az ad sp create-for-rbac -n "wwi-oss-sp" --role "Storage Blob Data Contributor" --scopes {adls-gen2-storage-account-resource-id}
   ```

   > **IMPORTANT**: You will need to replace the `{adls-gen2-storage-account-resource-id}` value with the resource ID of your ADLS Gen2 Storage account.

6. To retrieve the ADLS Gen2 Storage account resource ID you need to replace above, navigate to the resource group you created (`hands-on-lab-SUFFIX` where SUFFIX is your unique identifier).

7. In your hands-on-lab-SUFFIX resource group, select the ADLS Gen2 Storage account you provisioned previously, and on the ADLS Gen2 Storage account blade select **Properties** under **Settings** in the left-hand menu, and then select the copy to clipboard button to the right of the **Storage account resource ID** value.

   ![On the ADLS Gen2 Storage account blade, Properties is selected and highlighted in the left-hand menu, and the copy to clipboard button is highlighted next to Storage account resource ID.](media/adls-gen2-properties.png 'ADLS Gen2 Storage account properties')

8. Paste the Storage account resource ID into the command above, and then copy and paste the updated `az ad sp create-for-rbac` command at the Cloud Shell prompt and press `Enter`. The command should be similar to the following, with your subscription ID and resource group name:

   ```bash
   az ad sp create-for-rbac -n "wwi-oss-sp" --role "Storage Blob Data Contributor" --scope /subscriptions/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/resourceGroups/hands-on-lab/providers/Microsoft.Storage/storageAccounts/wwiadlsoss
   ```

   ![The az ad sp create-for-rbac command is entered into the Cloud Shell, and the output of the command is displayed.](media/azure-cli-create-sp.png 'Azure CLI')

9. Copy the output from the command into a text editor, as you will need it in the following steps. The output should be similar to:

   ```json
   {
     "appId": "16fae522-05f9-4e4d-8ccd-11db01909331",
     "displayName": "wwi-oss-sp",
     "name": "http://wwi-oss-sp",
     "password": "00713451-2b0e-416f-b5bf-fbb43c1836d5",
     "tenant": "d280491c-XXXX-XXXX-XXXX-XXXXXXXXXXXX"
   }
   ```

10. To verify the role assignment, select **Access control (IAM)** from the left-hand menu of the **ADLS Gen2 Storage account** blade, and then select the **Role assignments** tab and locate **wwi-oss-sp** under the _STORAGE BLOB DATA CONTRIBUTOR_ role.

    ![The Role assignments tab is displayed, with wwi-oss-sp account highlighted under STORAGE BLOB DATA CONTRIBUTOR role in the list.](media/storage-account-role-assignments.png 'Role assignments')

### Task 4: Add the service principal credentials and Tenant Id to Azure Key Vault

1. To provide access your ADLS Gen2 account from Azure Databricks you will use secrets stored in your Azure Key Vault account to provide the credentials of your newly created service principal within Databricks. Navigate to your Azure Key Vault account in the Azure portal, then select **Access Policies** and select the **+ Add new** button.

2. Choose the account that you are currently logged into the portal with as the principal and **check Select all** under `key permissions`, `secret permissions`, and `certificate permissions`, then click OK and then click **Save**.

3. Now select **Secrets** under Settings on the left-hand menu. On the Secrets blade, select **+ Generate/Import** on the top toolbar.

   ![Secrets is highlighted on the left-hand menu, and Generate/Import is highlighted on the top toolbar of the Secrets blade.](media/key-vault-generate-secret.png 'Key Vault secrets blade')

4. On the Create a secret blade, enter the following:

   - **Upload options**: Select Manual.
   - **Name**: Enter "WWI-SP-Client-ID".
   - **Value**: Paste the **appId** value from the Azure CLI output you copied in an earlier step.

   ![The Create a secret blade is displayed, with the previously mentioned values entered into the appropriate fields.](media/key-vault-create-wwi-sp-client-id-secret.png 'Create a secret')

5. Select **Create**.

6. Select **+ Generate/Import** again on the top toolbar to create another secret.

7. On the Create a secret blade, enter the following:

   - **Upload options**: Select Manual.
   - **Name**: Enter "WWI-SP-Client-Key".
   - **Value**: Paste the **password** value from the Azure CLI output you copied in an earlier step.

   ![The Create a secret blade is displayed, with the previously mentioned values entered into the appropriate fields.](media/key-vault-create-wwi-sp-client-key-secret.png 'Create a secret')

8. Select **Create**.

9. To perform authentication using the service principal account in Databricks you will also need to provide your Azure AD Tenant ID. Select **+ Generate/Import** again on the top toolbar to create another secret.

10. On the Create a secret blade, enter the following:

    - **Upload options**: Select Manual.
    - **Name**: Enter "Azure-Tenant-ID".
    - **Value**: Paste the **tenant** value from the Azure CLI output you copied in an earlier step.

    ![The Create a secret blade is displayed, with the previously mentioned values entered into the appropriate fields.](media/key-vault-create-azure-tenant-id-secret.png 'Create a secret')

11. Select **Create**.

### Task 5: Create an Azure Databricks cluster

In this task, you will connect to your Azure Databricks workspace and create a cluster to use for this hands-on lab.

1. Return to the [Azure portal](https://portal.azure.com), navigate to the Azure Databricks workspace located in your lab resource group, and select **Launch Workspace** from the overview blade, signing into the workspace with your Azure credentials, if required.

   ![The Launch Workspace button is displayed on the Databricks Workspace Overview blade.](media/databricks-launch-workspace.png 'Launch Workspace')

2. Select **Clusters** from the left-hand navigation menu, and then select **+ Create Cluster**.

   ![The Clusters option in the left-hand menu is selected and highlighted, and the Create Cluster button is highlighted on the clusters page.](media/databricks-clusters.png 'Databricks Clusters')

3. On the Create Cluster screen, enter the following:

   - **Cluster Name**: Enter a name for your cluster, such as lab-cluster.
   - **Cluster Mode**: Select Standard.
   - **Databricks Runtime Version**: Select Runtime: 5.3 (Scala 2.11, Spark 2.4.0).
   - **Python Version**: Select 3.
   - **Enable autoscaling**: Ensure this is checked.
   - **Terminate after XX minutes of inactivity**: Leave this checked, and the number of minutes set to 120.
   - **Worker Type**: Select Standard_DS4_v2.
     - **Min Workers**: Leave set to 2.
     - **Max Workers**: Leave set to 8.
   - **Driver Type**: Set to Same as worker.
   - Expand Advanced Options and enter the following into the Spark Config box:

     ```bash
     spark.databricks.delta.preview.enabled true
     ```

   ![The Create Cluster screen is displayed, with the values specified above entered into the appropriate fields.](media/databricks-create-new-cluster.png 'Create a new Databricks cluster')

4. Select **Create Cluster**. It will take 3-5 minutes for the cluster to be created and started.

### Task 6: Load lab notebooks into Azure Databricks

In this task, you will import the notebooks contained in the [Managed open source databases on Azure MCW GitHub repo](https://github.com/Microsoft/MCW-Managed-open-source-databases-on-Azure) into your Azure Databricks workspace.

1. Navigate to your Azure Databricks workspace in the Azure portal, and select **Launch Workspace** from the overview blade, signing into the workspace with your Azure credentials, if required.

   ![The Launch Workspace button is displayed on the Databricks Workspace Overview blade.](media/databricks-launch-workspace.png 'Launch Workspace')

2. Select **Workspace** from the left-hand menu, then select **Users** and select your user account (email address), and then select the down arrow on top of your user workspace and select **Import** from the context menu.

   ![The Workspace menu is highlighted in the Azure Databricks workspace, and Users is selected with the current user's account selected and highlighted. Import is selected in the user's context menu.](media/databricks-workspace-import.png 'Import files into user workspace')

3. Within the Import Notebooks dialog, select **URL** for Import from, and then paste the following into the box: `https://github.com/solliancenet/MCW-Managed-open-source-databases-on-Azure/blob/master/Hands-on%20lab/lab-files/OSSDatabases.dbc`

   ![The Import Notebooks dialog is displayed](media/databricks-import-notebooks.png 'Import Notebooks dialog')

4. Select **Import**.

5. You should now see a folder named **OSSDatabases** in your user workspace. This folder contains all of the notebooks you will use throughout this hands-on lab.

### Task 7: Configure Azure Databricks Key Vault-backed secrets

In this task, you will connect to your Azure Databricks workspace and configure Azure Databricks secrets to use your Azure Key Vault account as a backing store.

1. Return to the [Azure portal](https://portal.azure.com), navigate to your newly provisioned Key Vault account and select **Properties** on the left-hand menu.

2. Copy the **DNS Name** and **Resource ID** property values and paste them to Notepad or some other text application that you can reference later. These values will be used in the next section.

   ![Properties is selected on the left-hand menu, and DNS Name and Resource ID are highlighted to show where to copy the values from.](media/key-vault-properties.png 'Key Vault properties')

3. Navigate to the Azure Databricks workspace you provisioned above, and select **Launch Workspace** from the overview blade, signing into the workspace with your Azure credentials, if required.

   ![The Launch Workspace button is displayed on the Databricks Workspace Overview blade.](media/databricks-launch-workspace.png 'Launch Workspace')

4. In your browser's URL bar, append **#secrets/createScope** to your Azure Databricks base URL (for example, <https://westus.azuredatabricks.net#secrets/createScope>).

5. Enter `key-vault-secrets` for the name of the secret scope.

6. Select **Creator** within the Manage Principal drop-down to specify only the creator (which is you) of the secret scope has the MANAGE permission.

   > MANAGE permission allows users to read and write to this secret scope, and, in the case of accounts on the Azure Databricks Premium Plan, to change permissions for the scope.

   > Your account must have the Azure Databricks Premium Plan for you to be able to select Creator. This is the recommended approach: grant MANAGE permission to the Creator when you create the secret scope, and then assign more granular access permissions after you have tested the scope.

7. Enter the **DNS Name** (for example, <https://wwi-keyvault-oss.vault.azure.net/>) and **Resource ID** you copied earlier during the Key Vault creation step, for example: `/subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/resourcegroups/hands-on-lab/providers/Microsoft.KeyVault/vaults/wwi-keyvault-oss`.

   ![Create Secret Scope form](media/create-secret-scope.png 'Create Secret Scope')

8. Select **Create**.

After a moment, you will see a dialog verifying that the secret scope has been created.

## Exercise 3: Send clickstream data to Kafka and process it in real time

Duration: 20 minutes

In this exercise, you will configure and run the `KafkaProducer` application to send clickstream data to your Kafka endpoint provided by your event hub. Then, you will run a notebook with your new Azure Databricks cluster to process the streaming data in real time and write it to your PostgreSQL `events` table.

### Task 1: Configure the KafkaProducer application

1. Navigate to your lab files you extracted for this lab. They should be located in a folder named `MCW-Managed-open-source-databases-on-Azure-master` at the root directory of your hard drive (e.g. `C:\MCW-Managed-open-source-databases-on-Azure-master`).

2. Navigate to the following folder within: `\Hands-on lab\Resources\Apps`.

3. Open either the `Windows` folder, or the `Linux` folder, depending on your operating system.

   ![The lab files path is highlighted.](media/lab-files.png 'Lab files')

4. Within the folder, open the `appconfig.json` file in a text editor, such as Notepad.

   ![The appsettings.json file is highlighted.](media/lab-files-windows.png 'appsettings.json')

5. Paste your Event Hubs connection string you copied earlier in between the empty quotation marks next to **EVENTHUB_CONNECTIONSTRING**, then **save** the file.

   ![The appsettings.json file is displayed with the Event Hubs connection string value added.](media/app-settings.png 'Opened appsettings.json file')

### Task 2: Open notebook and process the streaming data

In this task, you will open a Databricks notebook and complete the instructions within.

6. Leave the folder open for now and navigate back to your Azure Databricks workspace. You will be instructed to run `KafkaProducer.exe` in this folder after you have completed some steps within the lab notebook.

7. Within your Azure Databricks workspace, select **Workspace** from the left-hand menu, then select **Users** and select your user account (email address). Now select the **OSSDatabases** folder and then select the **1-Consume-Kafka** notebook to open it.

   ![The 1-Consume-Kafka notebook is highlighted in the Databricks workspace.](media/databricks-open-consume-kafka-notebook.png 'Databricks workspace')

8. After opening the notebook, you need to attach your cluster. To do this, select **Detached** in the toolbar, then select your cluster. If your cluster is not running, you will need to start it.

   ![The Detached toolbar item is highlighted, and the lab-cluster is highlighted.](media/databricks-select-cluster.png 'Attach cluster')

9. You can execute each cell by selecting the **play button** on the upper-right portion of the cell, or you can click anywhere in the cell and execute it by entering **Ctrl+Enter** on your keyboard.

   ![The Play button is highlighted in the Databricks cell.](media/databricks-execute-cell.png 'Databricks cell')

10. After you have completed all the steps in the notebook, continue to the next exercise.

## After the hands-on lab

Duration: X minutes

\[insert your custom Hands-on lab content here . . .\]

### Task 1: Task name

1.  Number and insert your custom workshop content here . . .

    a. Insert content here

        i.

### Task 2: Task name

1.  Number and insert your custom workshop content here . . .

        a.  Insert content here

            i.

    You should follow all steps provided _after_ attending the Hands-on lab.
