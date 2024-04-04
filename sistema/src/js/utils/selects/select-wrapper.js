import $ from "jquery";
import "select2/dist/css/select2.min.css";
import "select2";
import "select2-bootstrap-theme/dist/select2-bootstrap.min.css";
import selectLanguage from "select2/dist/js/i18n/pt-BR.js";

$.fn.select2.defaults.set("language", selectLanguage);

class SelectWrapper {
  constructor(
    selector,
    config = {
      options: null,
      onOpened: async () => await Promise.resolve([]),
      onSelected: () => { },
      onUnSelected: () => { },
      onChange: () => { },
      onBlur: () => { },
      markOptionAsSelectable: () => {
        return false;
      },
      parentId: null,
      sortable: true,
      loadOptionsOnStart: false,
      onOrdenable: () => { },
    }
  ) {
    this.selector = selector;
    this.config = config;
    this.parentId = config.parentId;
    this.hasCurrentValue = false;
    this.loaded = false;
    this.defaultOptions = {
      language: "pt-BR",
      minimumResultsForSearch: 0,
      closeOnSelect: false,
      theme: "bootstrap",
      placeholder: "Selecione",
      selectOnClose: false,
    };

    if ($(this.selector).attr("multiple")) {
      this.config.placeholder = "Selecione um ou mais";
      this.config.multiple = true;
    }

    if (this.parentId) {
      $(this.selector).attr("disabled", true);

      $(this.parentId).on("select2:select", () => {
        const data = $(this.parentId).find(":selected");
        $(this.selector).attr(
          "disabled",
          data && data.length > 0 ? false : true
        );
      });
    }

    this.options = { ...this.defaultOptions, ...config.options };

    this.initialize();
  }

  initialize() {
    const config = this.config;
    const selector = this.selector;
    const context = this;

    $(this.selector)
      .select2(this.options)
      .on("select2:opening", async () => {
        await context.load(config, selector, context);
      })
      .on("select2:select", (e) => {
        const item = e.params?.data;

        if (config.onSelected) {
          config.onSelected(item);
        }

        if (config.onChange) {
          config.onChange(item);
        }

        context.config.multiple && $(selector).select2("open");
      })
      .on("select2:unselect", (e) => {
        const item = e.params.data;

        if (config.onUnSelected) {
          config.onUnSelected(item);
        }

        if (config.onChange) {
          config.onChange(item);
        }
      })
      .on("select2:close", (e) => {
        const item = e.params.data;

        if (config.onBlur) {
          config.onBlur(item);
        }
      });

    if (this.parentId) {
      $(this.parentId).on("change", () => {
        $(selector).empty().select2({ data: [] });
        context.loaded = false;
      });
    }

    if (config.loadOptionsOnStart) {
      new Promise((resolve) => setTimeout(resolve, 1000)).then(() => {
        this.load(config, selector, context);
      });
    }

    this.loadOptions = async () => {
      return await this.load(config, selector, context, true);
    };
  }

  async reload() {
    const context = this;
    $(this.selector).empty().select2({ data: [] });
    context.loaded = false;
  }

  async load(config, selector, context, force = false) {
    if (
      config.onOpened === null ||
      ($(selector)?.select2("data")?.length > 0 && !context.hasCurrentValue) ||
      context.loaded
    ) {
      return;
    }

    const currentValue = $(selector)?.val();

    const parentIdValue = context.parentId ? $(context.parentId).val() : null;

    const result = await config.onOpened(parentIdValue);

    let data =
      currentValue instanceof Array
        ? result?.map((item) => ({
          id: item.id,
          text: item.description,
          selected:
            currentValue.some(
              (currentValueId) => currentValueId == item.id
            ) ||
            (config.markOptionAsSelectable &&
              config.markOptionAsSelectable(item)),
        })) ?? []
        : result?.map((item) => ({
          id: item.id,
          text: item.description,
          selected:
            item.id == currentValue ||
            (config.markOptionAsSelectable &&
              config.markOptionAsSelectable(item)),
        })) ?? [];

    if (config.sortable) {
      data = data?.sort((a, b) => a?.text?.localeCompare(b?.text));
    }

    if (this.options.allowEmpty) {
      data.unshift({
        id: "",
        text: "---",
        selected:
          currentValue == "" ||
          (config.markOptionAsSelectable &&
            config.markOptionAsSelectable(null)),
      });
    }

    context.setValue(data);

    context.hasCurrentValue = false;
    context.loaded = true;

    await new Promise((resolve) => setTimeout(resolve, 50));

    if (!force) {
      $(selector).select2("open");
    }

    if (force) {
      $(selector).trigger("select2:select");
    }

    return result;
  }

  setValue(data) {
    if ($(this.selector).data("select2")) {
      if (this.config.multiple) {
        $(this.selector).empty().select2({ data: data });

        return;
      }

      const group = {
        children: data,
        text: this.options.placeholder,
      };

      $(this.selector)
        .empty()
        .select2({ data: [group] });
      $(this.selector).trigger("change.select2");
    }
  }

  setCurrentValue(value) {
    if (value) {
      if (value instanceof Array) {
        value.forEach((item) => {
          $(this.selector).append(
            new Option(item?.description, item?.id, true, true)
          );
        });

        if (this.config.parentId) {
          setTimeout(() => {
            $(this.selector).val(value.map((item) => item?.id));
          }, 1000);
        } else {
          $(this.selector).val(value.map((item) => item?.id));
        }
      } else if (value) {
        this.setValue([
          {
            id: value.id,
            text: value.description,
            selected: true,
            defaultSelected: true,
          },
        ]);
      }
    }

    this.hasCurrentValue = true;
    this.loaded = false;
  }

  setCurrentId(value) {
    if (!value || !this.loaded) {
      return;
    }

    value = value instanceof Array ? value : [value];

    $(this.selector).val(value).trigger("select2:select");
  }

  getValue() {
    return $(this.selector).val();
  }

  getSelectedOptions() {
    return $(this.selector)
      .select2("data")
      ?.map(({ id, text }) => {
        return {
          id,
          description: text,
        };
      });
  }

  clear() {
    this.hasCurrentValue = false;
    this.loaded = false;
    $(this.selector).empty().select2({ data: [] }).trigger("change");
  }

  disable() {
    $(this.selector).prop("disabled", true);
  }

  enable() {
    $(this.selector).prop("disabled", false);
  }

  isEnable() {
    return !$(this.selector).prop("disabled");
  }

  addOption(option, selected = false) {
    if (!option) {
      return;
    }

    if ($(this.selector).find("option[value='" + option.id + "']").length) {
      if (selected) {
        $(this.selector).val(option.id).trigger("change");
      }
      return;
    }

    $(this.selector).append(
      new Option(option?.description, option?.id, selected, selected)
    );
  }

  removeOption(id) {
    $(this.selector).find(`option[value="${id}"]`).remove();
  }

  multiple() {
    $(this.selector).attr("multiple", true);
  }

  single() {
    $(this.selector).attr("multiple", false);
  }

  hasValue() {
    return $(this.selector).val() ? true : false;
  }

  getValues() {
    const value = $(this.selector).val();

    if (!value) {
      return [];
    }

    return Array.isArray(value) ? value : [value];
  }
}

export default SelectWrapper;
