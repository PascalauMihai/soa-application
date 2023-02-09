<template>
  <div>
    <b-row>
      <b-alert v-model="showSuccessAlert" variant="success" dismissible>
        {{ alertMessage }}
      </b-alert>
    </b-row>
    <b-row>
      <items-overview
        :totalElements="numberOfElements"
        totalItemsNames="Total Commands"
      ></items-overview>
    </b-row>
    <b-row class="mt-3">
      <b-card>
        <b-row align-h="between">
          <b-col cols="6">
            <h3>{{ tableHeader }}</h3>
          </b-col>
          <b-col cols="2">
            <b-row>
              <b-row>
                <span class="h6">PlatformId: {{ this.platformId }}</span>
                <hr />
                <input
                  style="
                    border-color: 20px black;
                    border-radius: 20px;
                    margin-right: 20px;
                  "
                  v-model="platformId"
                  placeholder="Set platform id"
                />
                <hr />
              </b-row>
              <b-col>
                <b-button
                  variant="primary"
                  id="show-btn"
                  @click="getCommandsData"
                >
                  <span class="h6 text-white">Get Commands for Platform</span>
                </b-button>
              </b-col>
            </b-row>
          </b-col>
        </b-row>
        <b-row class="mt-3">
          <b-table
            striped
            hover
            :items="items"
            :fields="fields"
            class="text-center"
          >
            <template #cell(actions)="data">
              <b-row>
                <b-col cols="7">
                  <b-icon-pencil-square
                    class="action-item"
                    variant="primary"
                    @click="getRowData(data.item.id)"
                  ></b-icon-pencil-square>
                </b-col>
                <b-col cols="1">
                  <b-icon-trash-fill
                    class="action-item"
                    variant="danger"
                    @click="showDeleteModal(data.item.id)"
                  ></b-icon-trash-fill>
                </b-col>
              </b-row>
            </template>
          </b-table>
        </b-row>
      </b-card>
    </b-row>

    <!-- Modal for adding new commands -->
    <b-modal
      ref="create-command-modal"
      size="xl"
      hide-footer
      title="New Command"
    >
      <create-command-form
        @closeCreateModal="closeCreateModal"
        @reloadDataTable="getCommandsData"
        @showSuccessAlert="showAlertCreate"
      ></create-command-form>
    </b-modal>
  </div>
</template>

<script>
import axios from "axios";
import ItemsOverview from "@/components/ItemsOverview.vue";
import CreateCommandForm from "@/components/CreateCommandForm.vue";

export default {
  components: {
    ItemsOverview,
    CreateCommandForm,
  },
  data() {
    return {
      // Note 'isActive' is left out and will not appear in the rendered table
      platformId: 1,
      fields: [
        {
          key: "id",
          label: "Id",
          sortable: false,
        },
        {
          key: "howTo",
          label: "How to",
          sortable: true,
        },
        {
          key: "commandLine",
          label: "Command Line",
          sortable: false,
        },
        {
          key: "cost",
          label: "Cost",
          sortable: false,
        },
        "actions",
      ],
      items: [],
      numberOfElements: 0,
      commandId: 0,
      companySearchTerm: "",
      tableHeader: "",
      showSuccessAlert: false,
      alertMessage: "",
    };
  },
  mounted() {
    this.getCommandsData();
  },
  methods: {
    showCreateModal() {
      this.$refs["create-command-modal"].show();
    },
    closeCreateModal() {
      this.$refs["create-command-modal"].hide();
    },
    getCommandsData() {
      axios
        .get(
          `http://soa-project.com/api/c/platforms/${this.platformId}/commands`
        )
        .then((response) => {
          console.log(response);
          this.tableHeader = "Commands (Commands DB)";
          this.items = response.data;
          this.numberOfElements = response.data.length;
        })
        .catch((error) => {
          this.items = [];
          this.numberOfElements = 0;
          console.log(error);
        });
    },
    getRowData(id) {
      this.$refs["edit-command-modal"].show();
      this.commandId = id;
    },
    closeEditModal() {
      this.$refs["edit-command-modal"].hide();
    },
    setCommandsTabActive() {
      this.tableHeader = "Commands";
      this.getCommandsData();
    },
    showAlertCreate() {
      this.showSuccessAlert = true;
      this.alertMessage = "Command was created successfully!";
    },
    showAlertUpdate() {
      this.showSuccessAlert = true;
      this.alertMessage = "Command was updated successfully";
    },
    showDeleteModal(id) {
      this.$refs["delete-command-modal"].show();
      this.commandId = id;
    },
    closeDeleteModal() {
      this.$refs["delete-command-modal"].hide();
    },
    showDeleteSuccessModal() {
      this.showSuccessAlert = true;
      this.alertMessage = "Command was deleted successfully!";
    },
  },
};
</script>

<style>
.action-item:hover {
  cursor: pointer;
}
</style>
