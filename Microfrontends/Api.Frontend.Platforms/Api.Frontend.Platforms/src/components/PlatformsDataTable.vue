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
        totalItemsNames="Total Platforms"
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
              <b-col>
                <b-button
                  variant="primary"
                  id="show-btn"
                  @click="showCreateModal"
                >
                  <b-icon-plus class="text-white"></b-icon-plus>
                  <span class="h6 text-white">New Platform</span>
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

    <!-- Modal for adding new platforms -->
    <b-modal
      ref="create-platform-modal"
      size="xl"
      hide-footer
      title="New Platform"
    >
      <create-platform-form
        @closeCreateModal="closeCreateModal"
        @reloadDataTable="getPlatformsData"
        @showSuccessAlert="showAlertCreate"
      ></create-platform-form>
    </b-modal>

    <!-- Modal for updating platforms -->
    <b-modal
      ref="edit-platform-modal"
      size="xl"
      hide-footer
      title="Edit platform"
    >
      <edit-platform-form
        @closeEditModal="closeEditModal"
        @reloadDataTable="getPlatformsData"
        @showSuccessAlert="showAlertUpdate"
        :platformId="platformId"
      ></edit-platform-form>
    </b-modal>

    <!-- Delete platform Modal -->
    <b-modal
      ref="delete-platform-modal"
      size="md"
      hide-footer
      title="Confirm Deletion"
    >
      <delete-platform-modal
        @closeDeleteModal="closeDeleteModal"
        @reloadDataTable="getPlatformsData"
        @showDeleteAlert="showDeleteSuccessModal"
        :platformId="platformId"
      ></delete-platform-modal>
    </b-modal>
  </div>
</template>

<script>
import axios from "axios";
import ItemsOverview from "@/components/ItemsOverview.vue";
import CreatePlatformForm from "@/components/CreatePlatformForm.vue";
import EditPlatformForm from "@/components/EditPlatformForm.vue";
import DeletePlatformModal from "@/components/DeletePlatformModal.vue";

export default {
  components: {
    ItemsOverview,
    CreatePlatformForm,
    EditPlatformForm,
    DeletePlatformModal,
  },
  data() {
    return {
      // Note 'isActive' is left out and will not appear in the rendered table

      fields: [
        {
          key: "id",
          label: "Id",
          sortable: false,
        },
        {
          key: "name",
          label: "Name",
          sortable: true,
        },
        {
          key: "publisher",
          label: "Publisher",
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
      platformId: 0,
      companySearchTerm: "",
      tableHeader: "",
      showSuccessAlert: false,
      alertMessage: "",
    };
  },
  mounted() {
    this.getPlatformsData();
  },
  methods: {
    showCreateModal() {
      this.$refs["create-platform-modal"].show();
    },
    closeCreateModal() {
      this.$refs["create-platform-modal"].hide();
    },
    getPlatformsData() {
      axios
        .get("http://soa-project.com/api/platforms/")
        .then((response) => {
          console.log(response);
          this.tableHeader = "Platforms";
          this.items = response.data;
          this.numberOfElements = response.data.length;
        })
        .catch((error) => {
          console.log(error);
        });
    },
    getCommandsData() {
      axios
        .get("http://soa-project.com/api/c/platforms/")
        .then((response) => {
          console.log(response);
          this.tableHeader = "Commands Platforms";
          this.items = response.data;
          this.numberOfElements = response.data.length;
        })
        .catch((error) => {
          console.log(error);
        });
    },
    getRowData(id) {
      this.$refs["edit-platform-modal"].show();
      this.platformId = id;
    },
    closeEditModal() {
      this.$refs["edit-platform-modal"].hide();
    },
    setPlatformsTabActive() {
      this.tableHeader = "Platforms";
      this.getPlatformsData();
    },
    showAlertCreate() {
      this.showSuccessAlert = true;
      this.alertMessage = "Platform was created successfully!";
    },
    showAlertUpdate() {
      this.showSuccessAlert = true;
      this.alertMessage = "Platform was updated successfully";
    },
    showDeleteModal(id) {
      this.$refs["delete-platform-modal"].show();
      this.platformId = id;
    },
    closeDeleteModal() {
      this.$refs["delete-platform-modal"].hide();
    },
    showDeleteSuccessModal() {
      this.showSuccessAlert = true;
      this.alertMessage = "Platform was deleted successfully!";
    },
  },
};
</script>

<style>
.action-item:hover {
  cursor: pointer;
}
</style>
